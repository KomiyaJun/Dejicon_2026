// PhotoLinkHandler.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MyGame.AudioSetting;

public class PhotoLinkHandler : MonoBehaviour
{
    [Header("クリック時の演出")]
    // クリック時に暗くする時間
    [SerializeField] private float flashDuration = 0.15f;
    // クリック時の暗さ
    [SerializeField] private Color clickedColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("ウィンドウ")]
    // メモ用ウィンドウのWindowData
    [SerializeField] private WindowData memoWindowData;
    // マップ用ウィンドウのWindowData
    [SerializeField] private WindowData mapWindowData;
    // その他用ウィンドウのWindowData
    [SerializeField] private WindowData defaultWindowData;
    // WindowMemo.Instance が null の場合に待機するタイムアウト秒数
    [SerializeField] private float waitTimeout = 5.0f;

    [Header("音関連")]
    [SerializeField] private SoundData clickData;

    // 画像のImageコンポーネント
    private Image photoImage;

    // クリックしたときに発火するリンクID
    private string linkID;

    private void Awake()
    {
        photoImage = GetComponent<Image>();
    }

    /// <summary>Window_Parent を毎回動的に取得して返す</summary>
    private Transform GetWindowParent()
    {
        GameObject obj = GameObject.Find("Window_Parent");
        if (obj != null) return obj.transform;
        Debug.LogWarning("[PhotoLinkHandler] Window_Parent が見つかりません");
        return null;
    }

    // PostItemViewから呼ばれてリンクIDをセットする
    public void SetLinkID(string id)
    {
        linkID = id;

        // リンクIDが空の場合はButtonを無効化する
        Button button = GetComponent<Button>();
        if (button != null)
            button.interactable = !string.IsNullOrEmpty(linkID);
    }

    // ButtonのクリックイベントのメソッドとしてPostItemViewから登録される
    public void OnPhotoClicked()
    {
        // リンクIDが空の場合は何もしない
        if (string.IsNullOrEmpty(linkID)) return;

        StartCoroutine(FlashAndOpen());
    }

    // クリック時に一瞬暗くしてからウィンドウを開くコルーチン
    private IEnumerator FlashAndOpen()
    {
        // 一瞬暗くする
        if (photoImage != null)
            photoImage.color = clickedColor;

        yield return new WaitForSeconds(flashDuration);

        // 元の色に戻す
        if (photoImage != null)
            photoImage.color = Color.white;

        // リンクIDによって処理を分岐
        OnLinkClicked(linkID);
    }

    // WindowDataからウィンドウを開くメソッド
    private void OpenWindow(WindowData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[PhotoLinkHandler] WindowData が null です");
            return;
        }

        Transform parent = GetWindowParent();

        if (WindowCache.Instance != null)
        {
            WindowCache.Instance.OpenWindow(data, parent);
        }
        else if (WindowService.Instance != null)
        {
            // WindowCacheがなければWindowManagerで開く（フォールバック）
            WindowService.Instance.OpenWindow(data);
        }
        else
        {
            Debug.LogWarning("[PhotoLinkHandler] WindowCache も WindowService も見つかりません");
        }
    }

    // ウィンドウを開いた後にキーワードを活性化するメソッド
    private void OpenWindowAndActivate(WindowData data, string key)
    {
        // すでに開いている場合はActivateContentだけ呼ぶ
        if (WindowCache.Instance != null && WindowCache.Instance.IsOpen(data))
        {
            WindowBase window = WindowCache.Instance.GetWindow(data);
            window?.transform.SetAsLastSibling();
            StartCoroutine(WaitForWindowMemoAndActivate(key));
            return;
        }

        // 閉じている場合は開いてからキーワードを活性化
        OpenWindow(data);
        StartCoroutine(WaitForWindowMemoAndActivate(key));
    }

    /// <summary>
    /// WindowMemo.Instance が有効になるまでポーリングしてから ActivateContent を呼ぶ。
    /// アニメーション時間(1秒)が経過すると OnOpen() が呼ばれて Instance がセットされる。
    /// waitTimeout 秒以内に Instance が取得できなければ警告してbreak。
    /// </summary>
    private IEnumerator WaitForWindowMemoAndActivate(string key)
    {
        float elapsed = 0f;

        // WindowMemo.Instance がセットされるまで待つ
        while (WindowMemo.Instance == null && elapsed < waitTimeout)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        if (WindowMemo.Instance == null)
        {
            Debug.LogWarning($"[PhotoLinkHandler] WindowMemo.Instance が {waitTimeout}秒 経っても取得できませんでした。キー: {key}");
            yield break;
        }

        // キーワードを活性化
        WindowMemo.Instance.ActivateContent(key);
        Debug.Log($"[PhotoLinkHandler] {key} を活性化しました");
    }

    // リンクIDによってウィンドウを開くメソッド
    private void OnLinkClicked(string id)
    {
        // 複合 ID: "+" で区切られた場合は各部分を再帰的に処理
        // 例: "memo_sakura+map_桜自宅" → メモとマップを同時起動
        if (id.Contains("+"))
        {
            string[] parts = id.Split('+');
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    OnLinkClicked(trimmed);
            }
            return;
        }

        // "memo_" で始まる場合はメモウィンドウを開いてキーワード解放
        if (id.StartsWith("memo_"))
        {
            string key = id.Replace("memo_", "");
            Debug.Log("[PhotoLinkHandler] メモへのジャンプ: " + key);
            OpenWindowAndActivate(memoWindowData, key);
        }
        // "map_" で始まる場合はマップウィンドウを開く
        else if (id.StartsWith("map_"))
        {
            string key = id.Replace("map_", "");
            Debug.Log("[PhotoLinkHandler] マップへのジャンプ: " + key);

            // SNS 経由でピンを公開（MapPinDatabase に記録し、マップ再表示時も復元）
            MapPinDatabase.Instance?.RevealPin(key);

            // マップウィンドウを開く
            OpenWindow(mapWindowData);

            // 航空/農耕マップ切り替え（kouku/nokou/toggle キーの場合のみ有効）
            StartCoroutine(SwitchMapAfterWindowOpens(key));
        }
        // 以外のリンク
        else
        {
            Debug.Log("[PhotoLinkHandler] 不明なクリック: " + id);
            OpenWindow(defaultWindowData);
        }

        PlaySE(clickData);
    }

    /// <summary>
    /// MapKoukuController.ActiveInstance が有効になるまで待ってから航空/農耕を切り替える。
    /// kouku/nokou/toggle 以外のキーはピンIDとして無視する（RevealPinで既に処理済み）。
    /// </summary>
    private IEnumerator SwitchMapAfterWindowOpens(string key)
    {
        // kouku / nokou / toggle 以外はマップ切り替えなし（ピン表示のみ）
        if (key != "kouku" && key != "nokou" && key != "toggle")
            yield break;

        float elapsed = 0f;

        // MapKoukuController.ActiveInstance がセットされるまで待つ
        while (MapKoukuController.ActiveInstance == null && elapsed < waitTimeout)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        if (MapKoukuController.ActiveInstance == null)
        {
            Debug.LogWarning("[PhotoLinkHandler] MapKoukuController.ActiveInstance が見つかりません");
            yield break;
        }

        // キーによってマップを切り替える
        switch (key)
        {
            case "kouku":
                MapKoukuController.ActiveInstance.ShowKoukuOn();
                Debug.Log("[PhotoLinkHandler] 校区線ありマップに切り替えました");
                break;
            case "nokou":
                MapKoukuController.ActiveInstance.ShowKoukuOff();
                Debug.Log("[PhotoLinkHandler] 校区線なしマップに切り替えました");
                break;
            case "toggle":
                MapKoukuController.ActiveInstance.ToggleKouku();
                Debug.Log("[PhotoLinkHandler] 校区線の表示を切り替えました");
                break;
        }
    }

    private void PlaySE(SoundData data)
    {
        SoundService.Instance.PlaySE(data);
    }
}