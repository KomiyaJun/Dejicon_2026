// PhotoLinkHandler.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PhotoLinkHandler : MonoBehaviour
{
    [Header("クリック時の演出")]
    // クリック時に暗くする時間
    [SerializeField] private float flashDuration = 0.15f;
    // クリック時の暗さ
    [SerializeField] private Color clickedColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("ウィンドウ")]
    [SerializeField] private WindowData memoWindowData;
    [SerializeField] private WindowData mapWindowData;
    [SerializeField] private WindowData defaultWindowData;
    [SerializeField] private float activateDelay = 2.0f;

    // ウィンドウを生成する親オブジェクト
    private Transform windowParent;

    // 画像のImageコンポーネント
    private Image photoImage;

    // ウィンドウキャッシュ
    private Dictionary<WindowData, WindowBase> windowCache
        = new Dictionary<WindowData, WindowBase>();

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

    // リンクIDによってウィンドウを開くメソッド
    private void OnLinkClicked(string id)
    {
        if (id.StartsWith("memo_"))
        {
            string key = id.Replace("memo_", "");
            Debug.Log("メモへのジャンプ: " + key);
            OpenWindowAndActivate(memoWindowData, key);
        }
        else if (id.StartsWith("map_"))
        {
            string key = id.Replace("map_", "");
            Debug.Log("マップへのジャンプ: " + key);
            OpenWindow(mapWindowData);
        }
        else
        {
            Debug.Log("リンクをクリック: " + id);
            OpenWindow(defaultWindowData);
        }
    }

    // ウィンドウを開くメソッド
    // WindowDataからウィンドウを開くメソッド
    private void OpenWindow(WindowData data)
    {
        if (data == null)
        {
            Debug.LogWarning("WindowDataがアサインされていません");
            return;
        }

        if (windowParent == null)
        {
            Debug.LogWarning("Window_Parentが見つかりません");
            return;
        }

        // キャッシュに存在する場合は再利用する
        if (windowCache.TryGetValue(data, out WindowBase cachedWindow))
        {
            // すでに開いている場合は最前面に移動するだけ
            if (cachedWindow.gameObject.activeSelf)
            {
                cachedWindow.transform.SetAsLastSibling();
                return;
            }

            // 閉じている場合は開く
            cachedWindow.Open();
            return;
        }

        // キャッシュにない場合は新規生成する
        GameObject obj = Instantiate(data.prefab, windowParent);
        WindowBase window = obj.GetComponent<WindowBase>();

        if (window == null)
        {
            Debug.LogError($"{data.prefab.name}にWindowBaseがアタッチされていません");
            return;
        }

        window.SetUpWindow(data);
        windowCache[data] = window;
        window.Open();
    }

    // ウィンドウを開いた後にキーワードを活性化するメソッド
    private void OpenWindowAndActivate(WindowData data, string key)
    {
        // すでに開いている場合はActivateContentだけ呼ぶ
        if (windowCache.TryGetValue(data, out WindowBase cachedWindow))
        {
            if (cachedWindow.gameObject.activeSelf)
            {
                // 最前面に移動
                cachedWindow.transform.SetAsLastSibling();
                // すぐにキーワードを活性化
                StartCoroutine(ActivateAfterDelay(key));
                return;
            }
        }

        // 閉じている場合は開いてからキーワードを活性化
        OpenWindow(data);
        StartCoroutine(ActivateAfterDelay(key));
    }

    // 指定秒数後にキーワードを活性化するコルーチン
    private IEnumerator ActivateAfterDelay(string key)
    {
        yield return new WaitForSeconds(activateDelay);

        if (WindowMemo.Instance == null)
        {
            Debug.LogWarning("WindowMemo.Instance が見つかりません");
            yield break;
        }

        WindowMemo.Instance.ActivateContent(key);
        Debug.Log($"{key} を活性化しました");
    }
}