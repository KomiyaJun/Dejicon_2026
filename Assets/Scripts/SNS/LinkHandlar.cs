using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LinkHandler : MonoBehaviour, IPointerClickHandler
{
    // クリック判定を行うTextMeshProUGUIコンポーネント
    [SerializeField] private TextMeshProUGUI targetText;

    [Header("クリック時の色")]
    // 通常時のリンク色（青）
    [SerializeField] private Color normalColor = new Color(0.29f, 0.56f, 0.85f);
    // クリック時のリンク色（白）
    [SerializeField] private Color clickedColor = new Color(1f, 1f, 1f);
    // 点滅時間（秒）
    [SerializeField] private float flashDuration = 0.15f;

    [Header("ウィンドウ")]
    // タグ用ウィンドウのWindowData
    [SerializeField] private WindowData tagWindowData;
    // プロフィール用ウィンドウのWindowData
    [SerializeField] private WindowData profileWindowData;
    // その他用ウィンドウのWindowData
    [SerializeField] private WindowData defaultWindowData;

    // ウィンドウを生成する親オブジェクト（Awakeで自動取得）
    private Transform windowParent;

    // WindowDataをキーにして生成済みウィンドウを管理する辞書
    // 2回目以降は同じインスタンスを再利用する
    private Dictionary<WindowData, WindowBase> windowCache
        = new Dictionary<WindowData, WindowBase>();

    private void Awake()
    {
        // Window_Parentを名前で自動取得する
        if (windowParent == null)
        {
            GameObject obj = GameObject.Find("Window_Parent");
            if (obj != null)
                windowParent = obj.transform;
            else
                Debug.LogWarning("Window_Parentが見つかりません");
        }
    }

    // uGUIからクリックイベントが呼ばれる
    public void OnPointerClick(PointerEventData eventData)
    {
        // クリックした座標からリンクのインデックスを取得
        // リンク上でなければ -1 が返る
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            targetText,
            eventData.position,
            eventData.pressEventCamera
        );

        // リンク以外をクリックした場合は何もしない
        if (linkIndex == -1) return;

        // リンク情報を取得してIDを取り出す
        TMP_LinkInfo linkInfo = targetText.textInfo.linkInfo[linkIndex];
        string linkID = linkInfo.GetLinkID();

        // 色を一瞬変えてからイベントを発火するコルーチンを開始
        StartCoroutine(FlashLink(linkIndex, linkID));
    }

    // クリック時に色を点滅させるコルーチン
    private IEnumerator FlashLink(int linkIndex, string linkID)
    {
        // クリック色に変更
        SetLinkColor(linkIndex, clickedColor);

        // flashDuration 秒待つ
        yield return new WaitForSeconds(flashDuration);

        // 元の色に戻す
        SetLinkColor(linkIndex, normalColor);

        // 色が戻ってからイベントを発火
        OnLinkClicked(linkID);
    }

    // 指定したリンクの全文字の頂点カラーを変更するメソッド
    private void SetLinkColor(int linkIndex, Color color)
    {
        TMP_LinkInfo linkInfo = targetText.textInfo.linkInfo[linkIndex];

        // リンク内の全文字をループして色を変更
        for (int i = linkInfo.linkTextfirstCharacterIndex;
             i < linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength;
             i++)
        {
            // 表示されない文字（スペース・改行など）はスキップ
            if (!targetText.textInfo.characterInfo[i].isVisible) continue;

            // 文字が属するメッシュのインデックスと頂点インデックスを取得
            int meshIndex = targetText.textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = targetText.textInfo.characterInfo[i].vertexIndex;

            // メッシュの頂点カラー配列を取得
            Color32[] colors = targetText.textInfo.meshInfo[meshIndex].colors32;

            // 1文字は4頂点で構成されるので4つ変更
            colors[vertexIndex + 0] = color;
            colors[vertexIndex + 1] = color;
            colors[vertexIndex + 2] = color;
            colors[vertexIndex + 3] = color;
        }

        // 変更した頂点カラーをメッシュに反映
        targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

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
            // 生成済みウィンドウを開く
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

        // ウィンドウのセットアップと表示
        window.SetUpWindow(data);

        // キャッシュに登録して次回以降再利用できるようにする
        windowCache[data] = window;

        window.Open();
    }

    // リンクIDによってイベントを分岐するメソッド
    private void OnLinkClicked(string linkID)
    {
        // "tag_" で始まる場合はタグウィンドウを開く
        if (linkID.StartsWith("tag_"))
        {
            string tag = linkID.Replace("tag_", "#");
            Debug.Log("タグをクリック: " + tag);
            OpenWindow(tagWindowData);
        }
        // "profile_" で始まる場合はプロフィールウィンドウを開く
        else if (linkID.StartsWith("profile_"))
        {
            string user = linkID.Replace("profile_", "@");
            Debug.Log("プロフィールをクリック: " + user);
            OpenWindow(profileWindowData);
        }
        // それ以外のリンク
        else
        {
            Debug.Log("リンクをクリック: " + linkID);
            OpenWindow(defaultWindowData);
        }
    }
}