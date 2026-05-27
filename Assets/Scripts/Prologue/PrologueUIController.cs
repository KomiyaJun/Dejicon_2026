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
    [SerializeField] private Image slideImage;
    [SerializeField] private TextMeshProUGUI slideText;
    [SerializeField] private GameObject skipHintObject;

    [Header("Fade Settings")]
    [SerializeField] private float imageFadeDuration = 0.8f;
    [SerializeField] private float textFadeDuration = 0.5f;

    [Header("Skip Hint Blink Settings")]
    [SerializeField, Range(0f, 1f)] private float blinkAlphaMin = 0.1f;
    [SerializeField, Range(0f, 1f)] private float blinkAlphaMax = 1.0f;
    [SerializeField] private float blinkSpeed = 2.0f;

    // ─────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────

    private TextMeshProUGUI skipHintText;
    private Coroutine blinkCoroutine;

    // タイピング中かどうかを外部から参照できるフラグ
    public bool IsTyping { get; private set; } = false;

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void Awake()
    {
        if (skipHintObject != null)
            skipHintText = skipHintObject.GetComponentInChildren<TextMeshProUGUI>();

        // 初期状態
        SetSkipHint(false);
        SetImageAlpha(0f);
        SetTextAlpha(0f);
    }

    // ─────────────────────────────────────────
    // Public API : 画像
    // ─────────────────────────────────────────

    /// <summary>画像をクロスフェードで切り替える</summary>
    public IEnumerator CrossFadeImage(Sprite newSprite)
    {
        if (slideImage == null) yield break;

        if (slideImage.color.a > 0f)
            yield return StartCoroutine(FadeImage(1f, 0f, imageFadeDuration));

        slideImage.sprite = newSprite;
        yield return StartCoroutine(FadeImage(0f, 1f, imageFadeDuration));
    }

    // ─────────────────────────────────────────
    // Public API : テキスト
    // ─────────────────────────────────────────

    /// <summary>
    /// テキストをタイプライター形式で表示する。
    /// 完了したら IsTyping = false になる。
    /// </summary>
    public IEnumerator PlayTyping(string fullText, float speed)
    {
        slideText.text = string.Empty;
        IsTyping = true;

        // テキストをフェードイン
        yield return StartCoroutine(FadeText(0f, 1f, textFadeDuration * 0.5f));

        // 1文字ずつ表示
        float interval = 1f / Mathf.Max(speed, 0.01f);
        foreach (char c in fullText)
        {
            slideText.text += c;
            yield return new WaitForSeconds(interval);
        }

        IsTyping = false;
    }

    /// <summary>タイピングを中断して全文を即時表示する</summary>
    public void CompleteTyping(string fullText)
    {
        StopAllCoroutines();
        blinkCoroutine = null;
        IsTyping = false;

        slideText.text = fullText;
        SetTextAlpha(1f);
        SetSkipHint(false); // 一瞬消してから再表示させる（Manager側でShowSkipHintを呼ぶ）
    }

    /// <summary>テキストとヒントをリセットする（次スライド準備）</summary>
    public void ResetText()
    {
        SetSkipHint(false);
        SetTextAlpha(0f);
        slideText.text = string.Empty;
        IsTyping = false;
    }

    // ─────────────────────────────────────────
    // Public API : Skip Hint
    // ─────────────────────────────────────────

    /// <summary>「クリックで次へ」ヒントを点滅表示する</summary>
    public void ShowSkipHint()
    {
        SetSkipHint(true);
    }

    /// <summary>「クリックで次へ」ヒントを非表示にする</summary>
    public void HideSkipHint()
    {
        SetSkipHint(false);
    }

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
        if (slideImage == null) return;
        Color c = slideImage.color;
        c.a = alpha;
        slideImage.color = c;
    }

    // ─────────────────────────────────────────
    // Private : Text
    // ─────────────────────────────────────────

    private IEnumerator FadeText(float from, float to, float duration)
    {
        if (slideText == null) yield break;
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
        if (slideText == null) return;
        slideText.alpha = alpha;
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