using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

public class EpilogueController : MonoBehaviour
{
    [Header("ビデオプレイヤー")]
    public VideoPlayer videoPlayer;

    [Header("「クリックでタイトルに戻る」のテキスト")]
    public TextMeshProUGUI returnText;

    [Header("テキストのフェード速度")]
    public float fadeSpeed = 2f;

    [Header("遷移先のタイトルシーン名")]
    public string titleSceneName = "Title";

    private bool canReturn = false;
    private bool isFading = false;

    private void Start()
    {
        // 初期状態ではテキストを非表示にする
        if (returnText != null)
        {
            returnText.gameObject.SetActive(false);
            Color c = returnText.color;
            c.a = 0f;
            returnText.color = c;
        }

        if (videoPlayer != null)
        {
            // 動画の終了イベントを登録
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("VideoPlayerが設定されていません。");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        canReturn = true;

        if (returnText != null)
        {
            // テキストを表示してフェード開始
            returnText.gameObject.SetActive(true);
            isFading = true;
        }
    }

    private void Update()
    {
        // フェードアニメーション（透明度を 0.1 〜 1.0 の間で往復させる）
        if (isFading && returnText != null)
        {
            Color c = returnText.color;
            // PingPongで0〜1を反復。下限を0.2くらいにして完全に見えなくなるのを防ぐ
            float alpha = Mathf.PingPong(Time.time * fadeSpeed, 0.8f) + 0.2f;
            c.a = alpha;
            returnText.color = c;
        }

        // 動画終了後、画面のどこかをクリックしたらタイトルへ遷移
        if (canReturn && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(titleSceneName);
        }
    }
}
