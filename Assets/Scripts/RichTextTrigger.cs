using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RichTextTrigger : MonoBehaviour
{
    [Header("エフェクト要素")]
    [SerializeField] private Image slashImage;          // ギザギザ斜線のImage（Filledに設定）
    [SerializeField] private TextMeshProUGUI targetText; // 表示したいテキスト
    [SerializeField] private RectTransform textRect;    // テキストのRectTransform（位置動着用）

    private Vector2 _originalTextPos;

    private void Awake()
    {
        // 最初の状態を記憶・初期化
        _originalTextPos = textRect.anchoredPosition;
        ResetState();
    }

    /// <summary>
    /// 演出の初期化
    /// </summary>
    private void ResetState()
    {
        slashImage.gameObject.SetActive(false);
        slashImage.fillAmount = 0f;

        // テキストは透明＆少し下に下げておく
        targetText.alpha = 0f;
        textRect.anchoredPosition = _originalTextPos + new Vector2(0, -20f);
    }

    /// <summary>
    /// リッチなテキスト出現アニメーションを再生
    /// </summary>
    public async UniTask PlayTextAnimationAsync(CancellationToken ct = default)
    {
        // 0. 状態リセット
        ResetState();

        // 1. 斜線を描画する
        slashImage.gameObject.SetActive(true);

        // 【修正】0.3s ではなく 0.3f に変更
        await slashImage.DOFillAmount(1f, 0.3f)
            .SetEase(Ease.OutQuad)
            .WithCancellation(ct);

        // 2. 斜線をフェードアウト ＆ 文字をフェードイン（同時に並行処理）
        // 【修正】0.4s, 0.5s などの記述をすべて f に変更
        await UniTask.WhenAll(
            // 斜線のフェードアウト
            slashImage.DOFade(0f, 0.4f)
                .SetEase(Ease.InQuad)
                .WithCancellation(ct),

            // 文字のフェードイン＋下から上にスッと移動
            targetText.DOFade(1f, 0.5f)
                .SetEase(Ease.OutCubic)
                .WithCancellation(ct),

            textRect.DOAnchorPos(_originalTextPos, 0.5f)
                .SetEase(Ease.OutCubic)
                .WithCancellation(ct)
        );

        // 3. 後片付け
        slashImage.gameObject.SetActive(false);
    }
}