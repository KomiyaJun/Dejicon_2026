using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// プロローグのUI描画に専念するクラス。
/// 入力の判断は PrologueManager が行い、このクラスは描画命令を受け取るだけ。
/// </summary>
public class PrologueUIController : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector 設定
    // ─────────────────────────────────────────

    [Header("UI References")]
    [SerializeField] private Image backgroundImage; // 背景画像（1枚）
    [SerializeField] private TextMeshProUGUI lineText;        // セリフテキスト
    [SerializeField] private GameObject skipHintObject;  // 「クリックで次へ」ヒント

    [Header("Fade Settings")]
    [SerializeField] private float imageFadeInDuration = 1.2f; // 背景フェードイン時間
    [SerializeField] private float textFadeDuration = 0.3f; // テキスト切り替えフェード時間

    [Header("Skip Hint Blink Settings")]
    [SerializeField, Range(0f, 1f)] private float blinkAlphaMin = 0.1f;
    [SerializeField, Range(0f, 1f)] private float blinkAlphaMax = 1.0f;
    [SerializeField] private float blinkSpeed = 2.0f;

    // ─────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────

    private TextMeshProUGUI skipHintText;
    private Coroutine blinkCoroutine;

    /// <summary>タイピング中かどうかを PrologueManager から参照するフラグ</summary>
    public bool IsTyping { get; private set; } = false;

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void Awake()
    {
        if (skipHintObject != null)
            skipHintText = skipHintObject.GetComponentInChildren<TextMeshProUGUI>();

        SetSkipHint(false);
        SetImageAlpha(0f);
        SetTextAlpha(0f);
    }

    // ─────────────────────────────────────────
    // Public API : 背景画像
    // ─────────────────────────────────────────

    /// <summary>背景画像をセットしてフェードインする（プロローグ開始時に1回だけ呼ぶ）</summary>
    public IEnumerator ShowBackground(Sprite sprite)
    {
        if (backgroundImage == null) yield break;
        backgroundImage.sprite = sprite;
        yield return StartCoroutine(FadeImage(0f, 1f, imageFadeInDuration));
    }

    // ─────────────────────────────────────────
    // Public API : テキスト行
    // ─────────────────────────────────────────

    /// <summary>
    /// 新しいセリフ行をタイプライター形式で表示する。
    /// 完了したら IsTyping = false になる。
    /// </summary>
    public IEnumerator PlayLine(string line, float speed)
    {
        // 前のテキストをフェードアウト
        yield return StartCoroutine(FadeText(lineText.alpha, 0f, textFadeDuration));

        lineText.text = string.Empty;
        IsTyping = true;

        // テキストをフェードイン
        yield return StartCoroutine(FadeText(0f, 1f, textFadeDuration));

        // 1文字ずつ表示
        float interval = 1f / Mathf.Max(speed, 0.01f);
        foreach (char c in line)
        {
            lineText.text += c;
            yield return new WaitForSeconds(interval);
        }

        IsTyping = false;
    }

    /// <summary>タイピングを中断して全文を即時表示する</summary>
    public void CompleteLine(string fullLine)
    {
        StopAllCoroutines();
        blinkCoroutine = null;
        IsTyping = false;

        lineText.text = fullLine;
        SetTextAlpha(1f);
        SetSkipHint(false); // Manager 側で ShowSkipHint を呼ぶ
    }

    /// <summary>テキストをリセットする（次行の準備）</summary>
    public void ResetLine()
    {
        SetSkipHint(false);
        SetTextAlpha(0f);
        lineText.text = string.Empty;
        IsTyping = false;
    }

    // ─────────────────────────────────────────
    // Public API : Skip Hint
    // ─────────────────────────────────────────

    public void ShowSkipHint() => SetSkipHint(true);
    public void HideSkipHint() => SetSkipHint(false);

    // ─────────────────────────────────────────
    // Private : Image
    // ─────────────────────────────────────────

    private IEnumerator FadeImage(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetImageAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetImageAlpha(to);
    }

    private void SetImageAlpha(float alpha)
    {
        if (backgroundImage == null) return;
        Color c = backgroundImage.color;
        c.a = alpha;
        backgroundImage.color = c;
    }

    // ─────────────────────────────────────────
    // Private : Text
    // ─────────────────────────────────────────

    private IEnumerator FadeText(float from, float to, float duration)
    {
        if (lineText == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetTextAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetTextAlpha(to);
    }

    private void SetTextAlpha(float alpha)
    {
        if (lineText == null) return;
        lineText.alpha = alpha;
    }

    // ─────────────────────────────────────────
    // Private : Skip Hint
    // ─────────────────────────────────────────

    private void SetSkipHint(bool visible)
    {
        if (skipHintObject == null) return;
        skipHintObject.SetActive(visible);

        if (visible)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkSkipHint());
        }
        else
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
        }
    }

    private IEnumerator BlinkSkipHint()
    {
        if (skipHintText == null) yield break;
        while (true)
        {
            float t = (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(blinkAlphaMin, blinkAlphaMax, t);
            skipHintText.alpha = alpha;
            yield return null;
        }
    }
}