using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面全体を管理するクラス。
/// BGMの再生、シーン遷移（フェード付き）を担当する。
/// </summary>
public class TitleManager : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector 設定
    // ─────────────────────────────────────────

    [Header("Scene Names")]
    [SerializeField] private string prologueSceneName = "Prologue"; // プロローグシーン名
    [SerializeField] private string creditSceneName   = "Credit";   // クレジットシーン名

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;   // BGM用AudioSource
    [SerializeField] private AudioClip  bgmClip;      // タイトルBGM

    [Header("Fade")]
    [SerializeField] private float fadeOutDuration = 1.0f;
    [SerializeField] private float fadeInDuration  = 1.0f;

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void Start()
    {
        PlayBGM();
        StartCoroutine(OpeningFadeIn());
    }

    // ─────────────────────────────────────────
    // Public Methods（TitleUIController から呼ばれる）
    // ─────────────────────────────────────────

    /// <summary>ゲームスタート：フェードしてプロローグへ</summary>
    public void OnStartGame()
    {
        StartCoroutine(TransitionToPrologue());
    }

    /// <summary>クレジット画面へ</summary>
    public void OnCredit()
    {
        StartCoroutine(TransitionToScene(creditSceneName));
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

    /// <summary>タイトル表示時のフェードイン</summary>
    private IEnumerator OpeningFadeIn()
    {
        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeIn(fadeInDuration));
    }

    /// <summary>プロローグシーンへフェード遷移</summary>
    private IEnumerator TransitionToPrologue()
    {
        yield return StartCoroutine(FadeOutAndLoad(prologueSceneName));
    }

    /// <summary>任意シーンへフェード遷移</summary>
    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return StartCoroutine(FadeOutAndLoad(sceneName));
    }

    /// <summary>フェードアウト → シーンロード</summary>
    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        // BGMをフェードアウトと同時に暗転
        StartCoroutine(FadeBGM(fadeOutDuration));

        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeOut(fadeOutDuration));

        SceneManager.LoadScene(sceneName);
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
        bgmSource.volume = startVolume; // 次回のために戻しておく
    }
}
