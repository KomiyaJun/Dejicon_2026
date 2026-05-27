using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面全体を管理するクラス。
/// BGMの再生、背景画像のフェードイン、シーン遷移（フェード付き）を担当する。
/// ※ タイトルテキスト「断片」は背景画像に描き込み済みのため、Textオブジェクト不要。
/// </summary>
public class TitleManager : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector 設定
    // ─────────────────────────────────────────

    [Header("Scene Names")]
    [SerializeField] private string prologueSceneName = "PrologueScene";
    [SerializeField] private string creditSceneName = "CreditScene";

    [Header("Background")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float bgFadeInDuration = 2.0f;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmClip;

    [Header("Fade")]
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] private float fadeInDuration = 1.0f;

    // TitleUIController に「背景フェードイン完了」を通知するためのイベント
    public event System.Action OnOpeningCompleted;

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void Start()
    {
        SetBackgroundAlpha(0f);
        PlayBGM();
        StartCoroutine(OpeningSequence());
    }

    // ─────────────────────────────────────────
    // Public Methods（TitleUIController から呼ばれる）
    // ─────────────────────────────────────────

    /// <summary>ゲームスタート：フェードしてプロローグへ</summary>
    public void OnStartGame()
    {
        StartCoroutine(FadeOutAndLoad(prologueSceneName));
    }

    /// <summary>クレジット画面へ</summary>
    public void OnCredit()
    {
        StartCoroutine(FadeOutAndLoad(creditSceneName));
    }

    /// <summary>ゲーム終了</summary>
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ─────────────────────────────────────────
    // Private Coroutines
    // ─────────────────────────────────────────

    /// <summary>
    /// オープニング演出：背景フェードイン完了後に
    /// TitleUIController へ「Press Any Key」フェーズ開始を通知する。
    /// </summary>
    private IEnumerator OpeningSequence()
    {
        // 画面フェードイン（黒→透明）と背景画像フェードインを並行実行
        StartCoroutine(FadeBackground(0f, 1f, bgFadeInDuration));

        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeIn(fadeInDuration));

        // 背景フェードインの残り時間を待つ
        float remaining = bgFadeInDuration - fadeInDuration;
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        // UIController に完了を通知 → PressAnyKey フェーズへ
        OnOpeningCompleted?.Invoke();
    }

    /// <summary>フェードアウト → シーンロード</summary>
    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        StartCoroutine(FadeBGM(fadeOutDuration));

        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeOut(fadeOutDuration));

        SceneManager.LoadScene(sceneName);
    }

    // ─────────────────────────────────────────
    // Background
    // ─────────────────────────────────────────

    private IEnumerator FadeBackground(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetBackgroundAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetBackgroundAlpha(to);
    }

    private void SetBackgroundAlpha(float alpha)
    {
        if (backgroundImage == null) return;
        Color c = backgroundImage.color;
        c.a = alpha;
        backgroundImage.color = c;
    }

    // ─────────────────────────────────────────
    // BGM
    // ─────────────────────────────────────────

    private void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    private IEnumerator FadeBGM(float duration)
    {
        if (bgmSource == null) yield break;

        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }
}