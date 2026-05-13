using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ScaleAnimator : MonoBehaviour
{
    [SerializeField] private AnimationObserver observer;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        if (observer != null)
            observer.OnScaleRequested += HandleScaleRequested;
    }

    private void OnDisable()
    {
        if (observer != null)
            observer.OnScaleRequested -= HandleScaleRequested;

        // オブジェクト破棄時にアニメーションをキャンセル
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void HandleScaleRequested(int targetScale)
    {
        // 以前のアニメーションをキャンセルして新しいものに上書き（Fire and Forget）
        ScaleAsync(targetScale).Forget();
    }

    private async UniTaskVoid ScaleAsync(int targetValue)
    {
        // 前回の処理をキャンセルして二重実行を防止
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            // DOTweenをUniTaskで待機可能にする
            await transform.DOScale(Vector3.one * targetValue, duration)
                .SetEase(easeType)
                .WithCancellation(token);

            Debug.Log($"[Animator] Scale to {targetValue} completed.");
        }
        catch (System.OperationCanceledException)
        {
            // キャンセル時の処理（必要であれば）
        }
    }
}