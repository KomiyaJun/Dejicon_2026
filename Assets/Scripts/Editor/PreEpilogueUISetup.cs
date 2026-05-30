using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class PreEpilogueUISetup : EditorWindow
{
    [MenuItem("Tools/Setup Pre-Epilogue UI")]
    public static void SetupUI()
    {
        // 1. 最前面に表示するための専用 Canvas 作成
        GameObject canvasObj = new GameObject("PreEpilogueCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // 最前面に表示されるように設定
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. 画面全体を覆う半透明の暗転パネルを作成
        GameObject panelObj = new GameObject("DarkOverlayPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f); // 黒の半透明
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        // パネルにコントローラーをアタッチ
        EpilogueTransitionController controller = panelObj.AddComponent<EpilogueTransitionController>();

        // 3. テキストボックスの枠を作成
        GameObject dialogBoxObj = new GameObject("DialogBox");
        dialogBoxObj.transform.SetParent(panelObj.transform, false);
        
        Image dialogBoxImage = dialogBoxObj.AddComponent<Image>();
        dialogBoxImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // 濃いグレー
        
        // 画面下部に配置
        RectTransform dialogRect = dialogBoxObj.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0f);
        dialogRect.anchorMax = new Vector2(0.5f, 0f);
        dialogRect.pivot = new Vector2(0.5f, 0f);
        dialogRect.anchoredPosition = new Vector2(0, 50); // 下から50px浮かせる
        dialogRect.sizeDelta = new Vector2(1400, 250); // 横幅1400, 高さ250

        // 4. セリフ用 TextMeshPro の作成
        GameObject textObj = new GameObject("DialogText");
        textObj.transform.SetParent(dialogBoxObj.transform, false);
        
        TextMeshProUGUI dialogTextUI = textObj.AddComponent<TextMeshProUGUI>();
        dialogTextUI.text = "すべての真実を知る覚悟はありますか？";
        dialogTextUI.fontSize = 45;
        dialogTextUI.alignment = TextAlignmentOptions.TopLeft;
        dialogTextUI.color = Color.white;
        dialogTextUI.margin = new Vector4(40, 40, 40, 40); // 余白を設定
        dialogTextUI.enableWordWrapping = true;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        // 5. クリック案内用 TextMeshPro の作成
        GameObject promptObj = new GameObject("ClickPromptText");
        promptObj.transform.SetParent(dialogBoxObj.transform, false);

        TextMeshProUGUI promptTextUI = promptObj.AddComponent<TextMeshProUGUI>();
        promptTextUI.text = "▼クリックで次へ";
        promptTextUI.fontSize = 30;
        promptTextUI.alignment = TextAlignmentOptions.BottomRight;
        promptTextUI.color = new Color(1f, 1f, 1f, 0.8f);
        promptTextUI.margin = new Vector4(0, 0, 30, 20); // 右下に余白

        RectTransform promptRect = promptObj.GetComponent<RectTransform>();
        promptRect.anchorMin = Vector2.zero;
        promptRect.anchorMax = Vector2.one;
        promptRect.sizeDelta = Vector2.zero;
        promptRect.anchoredPosition = Vector2.zero;

        // 6. コントローラーにテキストを紐づける
        controller.dialogTextUI = dialogTextUI;
        controller.clickPromptUI = promptTextUI;

        // Undo 登録（Ctrl+Zで戻せるようにする）
        Undo.RegisterCreatedObjectUndo(canvasObj, "Setup Pre-Epilogue UI");

        Debug.Log("エピローグ遷移用のワンクッションUIの生成が完了しました！");
    }
}
