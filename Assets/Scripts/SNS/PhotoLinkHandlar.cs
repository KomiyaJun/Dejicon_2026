// PhotoLinkHandler.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    // キーワード活性化までの待機秒数
    [SerializeField] private float activateDelay = 2.0f;

    // ウィンドウを生成する親オブジェクト
    private Transform windowParent;

    // 画像のImageコンポーネント
    private Image photoImage;

    // クリックしたときに発火するリンクID
    private string linkID;

    private void Awake()
    {
        photoImage = GetComponent<Image>();

        // Window_Parentを自動取得
        GameObject obj = GameObject.Find("Window_Parent");
        if (obj != null)
            windowParent = obj.transform;
        else
            Debug.LogWarning("Window_Parentが見つかりません");
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

    // Buttonのクリックイベントに登録するメソッド
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
        if (WindowCache.Instance == null)
        {
            Debug.LogWarning("WindowCacheが見つかりません");
            return;
        }

        WindowCache.Instance.OpenWindow(data, windowParent);
    }

    // ウィンドウを開いた後にキーワードを活性化するメソッド
    private void OpenWindowAndActivate(WindowData data, string key)
    {
        // すでに開いている場合はActivateContentだけ呼ぶ
        if (WindowCache.Instance != null && WindowCache.Instance.IsOpen(data))
        {
            WindowBase window = WindowCache.Instance.GetWindow(data);
            window?.transform.SetAsLastSibling();
            StartCoroutine(ActivateAfterDelay(key));
            return;
        }

        // 閉じている場合は開いてからキーワードを活性化
        OpenWindow(data);
        StartCoroutine(ActivateAfterDelay(key));
    }

    // 指定秒数後にキーワードを活性化するコルーチン
    private IEnumerator ActivateAfterDelay(string key)
    {
        // activateDelay 秒待つ
        yield return new WaitForSeconds(activateDelay);

        // WindowMemo のインスタンスが存在するか確認
        if (WindowMemo.Instance == null)
        {
            Debug.LogWarning("WindowMemo.Instance が見つかりません");
            yield break;
        }

        // キーワードを活性化
        WindowMemo.Instance.ActivateContent(key);
        Debug.Log($"{key} を活性化しました");
    }

    // リンクIDによってウィンドウを開くメソッド
    private void OnLinkClicked(string id)
    {
        // "memo_" で始まる場合はメモウィンドウを開いてキーワードを活性化
        if (id.StartsWith("memo_"))
        {
            string key = id.Replace("memo_", "");
            Debug.Log("メモへのジャンプ: " + key);
            OpenWindowAndActivate(memoWindowData, key);
        }
        // "map_" で始まる場合はマップウィンドウを開く
        else if (id.StartsWith("map_"))
        {
            string key = id.Replace("map_", "");
            Debug.Log("マップへのジャンプ: " + key);
            OpenWindow(mapWindowData);
        }
        // それ以外のリンク
        else
        {
            Debug.Log("リンクをクリック: " + id);
            OpenWindow(defaultWindowData);
        }
    }
}