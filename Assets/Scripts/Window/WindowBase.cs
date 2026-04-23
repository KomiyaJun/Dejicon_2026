using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Unity.Android.Gradle.Manifest;
using UnityEngine.EventSystems;

//ウィンドウの基幹スクリプト
//継承先でOnOpenやOnCloseをoverrideすることを想定した構造にしていますが、特に何も変えなくても使えます。
//担当者 : 小宮
public abstract class WindowBase : MonoBehaviour, IPointerDownHandler
{
    public System.Action<WindowBase> OnStateChanged;

    [Header("Animation Settings")]
    [SerializeField] protected float animationDuration = 1.0f;
    [SerializeField] protected Ease openEase = Ease.OutBack;
    [SerializeField] protected Ease closeEase = Ease.InBack;

    [Header("Label Settings")]
    [SerializeField] protected TextMeshProUGUI windowLabelText;
    [SerializeField] protected Image windowLabelImage;
    
    private Vector3 originalScale;
    private bool isInitialized = false;
    private bool isAnimating = false;

    //最初に一度だけ元のサイズを記録
    private void InitializeIfNeeded()
    {
        if (isInitialized) return;
        originalScale = transform.localScale;
        isInitialized = true;

        OnStateChanged?.Invoke(this);
    }

    //アプリ起動
    public async void Open()
    {
        if(isAnimating) return;

        InitializeIfNeeded();

        gameObject.SetActive(true);
        Debug.Log($"{gameObject.name}を開きました");

        isAnimating = true;

        transform.localScale = Vector3.zero;

        await transform.DOScale(originalScale, animationDuration)
    .SetEase(openEase)
    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        isAnimating = false;

        await OnOpen();

        OnStateChanged?.Invoke(this);
    }

    //アプリ終了
    public async void Close()
    {
        // アニメーション中、または既に非表示なら無視
        if (isAnimating || !gameObject.activeSelf) return;

        isAnimating = true;

        // 縮小アニメーション
        await transform.DOScale(Vector3.zero, animationDuration)
            .SetEase(closeEase)
            .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        await OnClose(); // メンバーが追加した処理を実行

        gameObject.SetActive(false);
        isAnimating = false;
    }

    private void SetWindowLabelText(WindowData data)
    {
        windowLabelText.text = data.windowName;
    }
    private void SetWindowLabelColor(WindowData data)
    {
        windowLabelImage.color = data.windowColor;
    }

    public void SetUpWindow(WindowData data)
    {
        SetWindowLabelColor(data);
        SetWindowLabelText(data);
    }

    // ウィンドウ内のどこかがクリックされたら呼ばれる
    public void OnPointerDown(PointerEventData eventData)
    {
        // 自分（ウィンドウ全体）を最前面へ
        transform.SetAsLastSibling();
        // ログを出しておくとデバッグが捗ります
        Debug.Log($"{gameObject.name} が最前面に移動しました");
    }

    //起動時、終了時に処理を追加したい場合以下の関数に処理を追加(継承先でoverride)してください
    protected virtual async UniTask OnOpen() => await UniTask.CompletedTask;
    protected virtual async UniTask OnClose() => await UniTask.CompletedTask; 
}
