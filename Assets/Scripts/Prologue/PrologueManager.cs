using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プロローグシーン全体を管理するクラス。
/// スライドの状態を enum で厳密に管理し、入力の判断をここに集約する。
/// </summary>
public class PrologueManager : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector 設定
    // ─────────────────────────────────────────

    [Header("Slides")]
    [SerializeField] private PrologueSlideData[] slides;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float bgmFadeDuration = 1.0f;

    [Header("Skip")]
    [Tooltip("クリック / タップでスライドをスキップできるか")]
    [SerializeField] private bool allowSkip = true;
    [Tooltip("Esc キーでプロローグ全体をスキップして GameScene に飛ぶか")]
    [SerializeField] private bool allowFullSkip = true;

    // ─────────────────────────────────────────
    // スライドの状態
    // ─────────────────────────────────────────

    private enum SlideState
    {
        None,           // 初期 / 処理なし
        ImageFading,    // 画像フェード中（入力を受け付けない）
        Typing,         // テキスト送り中（クリックで全文表示）
        WaitingForNext, // 全文表示済み・次クリック待ち（クリックで次スライドへ）
    }

    // ─────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────

    private PrologueUIController uiController;
    private SlideState slideState = SlideState.None;
    private bool isTransitioning = false;

    // クリック入力をフレーム跨ぎで安全に受け取るフラグ
    private bool clickConsumed = false;

    // 現在のスライドデータ（CompleteTyping 時に全文を渡すために保持）
    private PrologueSlideData currentSlide;

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void Awake()
    {
        uiController = GetComponent<PrologueUIController>();
        if (uiController == null)
            Debug.LogError("[PrologueManager] PrologueUIController が同一 GameObject に見つかりません。");
    }

    private void Start()
    {
        StartCoroutine(RunPrologue());
    }

    private void Update()
    {
        if (isTransitioning) return;

        // Esc でプロローグ全体スキップ
        if (allowFullSkip && Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(EndPrologue());
            return;
        }

        // クリック入力を記録（消費は RunPrologue 側で行う）
        if (allowSkip && Input.GetMouseButtonDown(0))
            clickConsumed = true;
    }

    // ─────────────────────────────────────────
    // Prologue Flow
    // ─────────────────────────────────────────

    private IEnumerator RunPrologue()
    {
        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeIn());

        for (int i = 0; i < slides.Length; i++)
        {
            if (isTransitioning) yield break;

            currentSlide = slides[i];
            clickConsumed = false;

            HandleBGM(currentSlide.bgm);

            // ── 1. 画像フェード ──────────────────────
            slideState = SlideState.ImageFading;
            uiController.ResetText();
            yield return StartCoroutine(uiController.CrossFadeImage(currentSlide.image));

            // ── 2. テキスト送り ──────────────────────
            slideState = SlideState.Typing;
            clickConsumed = false; // 画像フェード中の誤クリックを破棄

            Coroutine typingCoroutine = StartCoroutine(
                uiController.PlayTyping(currentSlide.text, currentSlide.textSpeed));

            // タイピング完了 or クリックを待つ
            while (uiController.IsTyping)
            {
                if (clickConsumed)
                {
                    clickConsumed = false;
                    // タイピング中断 → 全文即時表示
                    StopCoroutine(typingCoroutine);
                    uiController.CompleteTyping(currentSlide.text);
                    break;
                }
                yield return null;
            }

            // ── 3. 全文表示済み・次クリック待ち ──────
            slideState = SlideState.WaitingForNext;
            clickConsumed = false; // タイピング完了直後の誤クリックを破棄
            uiController.ShowSkipHint();

            // 自動進行 or クリック待ち
            if (currentSlide.autoAdvanceDelay > 0f)
            {
                yield return new WaitForSeconds(currentSlide.autoAdvanceDelay);
            }
            else
            {
                yield return StartCoroutine(WaitForClick());
            }

            uiController.HideSkipHint();
        }

        yield return StartCoroutine(EndPrologue());
    }

    /// <summary>クリックが来るまで待つ。isTransitioning になったら即抜ける。</summary>
    private IEnumerator WaitForClick()
    {
        clickConsumed = false;
        while (!clickConsumed && !isTransitioning)
            yield return null;
        clickConsumed = false;
    }

    // ─────────────────────────────────────────
    // Scene Transition
    // ─────────────────────────────────────────

    private IEnumerator EndPrologue()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        uiController.HideSkipHint();
        StartCoroutine(FadeBGM(bgmFadeDuration));

        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeOut());

        SceneManager.LoadScene(gameSceneName);
    }

    // ─────────────────────────────────────────
    // BGM
    // ─────────────────────────────────────────

    private void HandleBGM(AudioClip newClip)
    {
        if (bgmSource == null || newClip == null) return;
        if (bgmSource.clip == newClip && bgmSource.isPlaying) return;

        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    private IEnumerator FadeBGM(float duration)
    {
        if (bgmSource == null || !bgmSource.isPlaying) yield break;

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