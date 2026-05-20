using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトル画面のUIを管理するクラス。
/// ボタンのイベント登録と、ボタン操作の入力受付制御を担当する。
/// </summary>
public class TitleUIController : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector 設定
    // ─────────────────────────────────────────

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button quitButton;

    [Header("SE (任意)")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip   buttonClickSE;

    private TitleManager titleManager;
    private bool isTransitioning = false; // 多重押し防止フラグ

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void Awake()
    {
        titleManager = GetComponent<TitleManager>();
        if (titleManager == null)
        {
            Debug.LogError("[TitleUIController] TitleManager が見つかりません。同一GameObjectにアタッチしてください。");
        }
    }

    private void Start()
    {
        RegisterButtonEvents();
    }

    // ─────────────────────────────────────────
    // Button Registration
    // ─────────────────────────────────────────

    private void RegisterButtonEvents()
    {
        if (startButton  != null) startButton.onClick.AddListener(OnStartClicked);
        if (creditButton != null) creditButton.onClick.AddListener(OnCreditClicked);
        if (quitButton   != null) quitButton.onClick.AddListener(OnQuitClicked);
    }

    // ─────────────────────────────────────────
    // Button Callbacks
    // ─────────────────────────────────────────

    private void OnStartClicked()
    {
        if (!TryBeginTransition()) return;
        PlaySE();
        titleManager.OnStartGame();
    }

    private void OnCreditClicked()
    {
        if (!TryBeginTransition()) return;
        PlaySE();
        titleManager.OnCredit();
    }

    private void OnQuitClicked()
    {
        if (!TryBeginTransition()) return;
        PlaySE();
        titleManager.OnQuitGame();
    }

    // ─────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────

    /// <summary>遷移中の多重押しを防ぐ。遷移可能なら true を返す。</summary>
    private bool TryBeginTransition()
    {
        if (isTransitioning) return false;
        isTransitioning = true;
        SetButtonsInteractable(false);
        return true;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (startButton  != null) startButton.interactable  = interactable;
        if (creditButton != null) creditButton.interactable = interactable;
        if (quitButton   != null) quitButton.interactable   = interactable;
    }

    private void PlaySE()
    {
        if (seSource != null && buttonClickSE != null)
            seSource.PlayOneShot(buttonClickSE);
    }
}
