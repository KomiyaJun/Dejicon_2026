using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェードイン・アウトを管理するシングルトンクラス。
/// Canvas上のImageコンポーネント（黒パネル）を使って画面全体をフェードさせる。
/// </summary>
public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private Image fadePanel;         // 黒背景のUI Image
    [SerializeField] private float defaultDuration = 1.0f;

    private void Awake()
    {
        // シングルトン設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // シーンをまたいで維持

        // 起動時は透明にしておく
        if (fadePanel != null)
        {
            SetAlpha(0f);
        }
    }

    // ─────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────

    /// <summary>フェードイン（黒→透明）</summary>
    public IEnumerator FadeIn(float duration = -1f)
    {
        if (duration < 0f) duration = defaultDuration;
        yield return Fade(1f, 0f, duration);
    }

    /// <summary>フェードアウト（透明→黒）</summary>
    public IEnumerator FadeOut(float duration = -1f)
    {
        if (duration < 0f) duration = defaultDuration;
        yield return Fade(0f, 1f, duration);
    }

    // ─────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadePanel == null) yield break;

        fadePanel.raycastTarget = true; // フェード中はクリックをブロック
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }

        SetAlpha(to);
        fadePanel.raycastTarget = (to > 0.5f); // 透明なら入力を通す
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }
}
