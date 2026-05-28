using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プロローグシーン全体を管理するクラス。
/// 背景画像を1回表示し、セリフ行をクリック/エンターで順送りする。
/// </summary>
public class PrologueManager : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector 設定
    // ─────────────────────────────────────────

    [Header("Slide Data")]
    [SerializeField] private PrologueSlideData slideData; // スライドデータ（1つ）

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float bgmFadeDuration = 1.0f;

    [Header("Skip")]
    [Tooltip("クリック / エンターキーでセリフを送れるか")]
    [SerializeField] private bool allowSkip = true;
    [Tooltip("Esc キーでプロローグ全体をスキップして GameScene へ飛ぶか")]
    [SerializeField] private bool allowFullSkip = true;

    // ─────────────────────────────────────────
    // 行の状態
    // ─────────────────────────────────────────

    private enum LineState
    {
        ImageFading,    // 背景フェード中（入力を受け付けない）
        Typing,         // タイピング中（入力で全文表示）
        WaitingForNext, // 全文表示済み・次入力待ち
    }

    // ─────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────

    private PrologueUIController uiController;
    private LineState lineState = LineState.ImageFading;
    private bool isTransitioning = false;
    private bool inputReceived = false; // クリック or エンターの入力フラグ
    private string currentLine = string.Empty;

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

        // クリック or エンターキーで入力フラグを立てる
        // ※ ImageFading 中は Update で記録しても RunPrologue 側で無視する
        if (allowSkip &&
            (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            inputReceived = true;
        }
    }

    // ─────────────────────────────────────────
    // Prologue Flow
    // ─────────────────────────────────────────

    private IEnumerator RunPrologue()
    {
        if (slideData == null)
        {
            Debug.LogError("[PrologueManager] SlideData がアサインされていません。");
            yield break;
        }

        // タイトルからのフェードイン
        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeIn());

        // BGM 再生
        HandleBGM(slideData.bgm);

        // ── 1. 背景画像をフェードイン（1回だけ）──────
        lineState = LineState.ImageFading;
        yield return StartCoroutine(uiController.ShowBackground(slideData.backgroundImage));

        // 背景フェード中の誤入力を破棄
        inputReceived = false;

        // ── 2. セリフ行を順送り ──────────────────────
        string[] lines = slideData.lines;
        for (int i = 0; i < lines.Length; i++)
        {
            if (isTransitioning) yield break;

            currentLine = lines[i];
            inputReceived = false;

            // タイピング開始
            lineState = LineState.Typing;
            Coroutine typingCoroutine = StartCoroutine(
                uiController.PlayLine(currentLine, slideData.textSpeed));

            // タイピング完了 or 入力を待つ
            while (uiController.IsTyping)
            {
                if (inputReceived)
                {
                    inputReceived = false;
                    StopCoroutine(typingCoroutine);
                    uiController.CompleteLine(currentLine);
                    break;
                }
                yield return null;
            }

            // 全文表示済み・次入力待ち
            lineState = LineState.WaitingForNext;
            inputReceived = false; // タイピング完了直後の誤入力を破棄
            uiController.ShowSkipHint();

            yield return StartCoroutine(WaitForInput());

            uiController.HideSkipHint();
        }

        // 全セリフ終了 → GameScene へ
        yield return StartCoroutine(EndPrologue());
    }

    /// <summary>クリック / エンターが来るまで待つ</summary>
    private IEnumerator WaitForInput()
    {
        inputReceived = false;
        while (!inputReceived && !isTransitioning)
            yield return null;
        inputReceived = false;
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

        //SceneManager.LoadScene(gameSceneName);    トランジション仕様に置き換えます---小宮
        SceneTransitionManager.Instance.LoadScene(gameSceneName);
    }

    // ─────────────────────────────────────────
    // BGM
    // ─────────────────────────────────────────

    private void HandleBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
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