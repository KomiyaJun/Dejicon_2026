using UnityEngine;
using UnityEditor;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class EpilogueSetup : EditorWindow
{
    [MenuItem("Tools/Setup Epilogue Scene")]
    public static void SetupScene()
    {
        // 1. Controllerオブジェクトの作成
        GameObject epilogueObj = new GameObject("EpilogueController");
        EpilogueController controller = epilogueObj.AddComponent<EpilogueController>();

        // 2. VideoPlayerの設定
        VideoPlayer vp = epilogueObj.AddComponent<VideoPlayer>();
        vp.renderMode = VideoRenderMode.CameraNearPlane;
        vp.aspectRatio = VideoAspectRatio.FitInside;
        vp.targetCameraAlpha = 1f;
        vp.playOnAwake = true; // 自動再生
        vp.isLooping = false; // 1回だけ

        // 動画アセットを検索して割り当て
        VideoClip clip = AssetDatabase.LoadAssetAtPath<VideoClip>("Assets/Video/デジコン.mp4");
        if (clip != null)
        {
            vp.clip = clip;
            Debug.Log("動画ファイル『デジコン.mp4』を自動アタッチしました。");
        }
        else
        {
            Debug.LogWarning("動画ファイル『Assets/Video/デジコン.mp4』が見つかりませんでした。Inspectorから手動でセットしてください。");
        }
        
        controller.videoPlayer = vp;

        // カメラがなければ作成
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            // バックグラウンドを黒に
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = Color.black;
        }
        else
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = Color.black;
        }

        // 3. UI Canvas の作成
        GameObject canvasObj = new GameObject("EpilogueCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // 4. TextMeshPro の作成
        GameObject textObj = new GameObject("ReturnText");
        textObj.transform.SetParent(canvasObj.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "クリックでタイトルに戻る";
        text.fontSize = 50;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        // アンカーを画面下部中央に設定
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.1f);
        rect.anchorMax = new Vector2(0.5f, 0.1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(800, 100);

        controller.returnText = text;

        // 変更を保存対象にする
        Undo.RegisterCreatedObjectUndo(epilogueObj, "Setup Epilogue");
        Undo.RegisterCreatedObjectUndo(canvasObj, "Setup Epilogue");

        Debug.Log("エピローグシーンのセットアップが完了しました！");
    }
}
