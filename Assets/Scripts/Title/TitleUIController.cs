using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// タイトル画面のUIを管理するクラス。
/// 「PRESS ANY KEY」→ボタン表示 の2段階UI制御、
/// キーボードによるボタン選択・決定、ホバー演出を担当する。
/// </summary>
public class TitleUIController : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector 設定
    // ─────────────────────────────────────────

    [Header("Press Any Key UI")]
    [SerializeField] private GameObject pressAnyKeyObject;  // 「PRESS ANY KEY」のUI親オブジェクト
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;    // 点滅させるテキスト
    [SerializeField] private float pakBlinkSpeed = 1.0f;  // 点滅速度
    [SerializeField] private float pakAlphaMin = 0.0f; // 点滅最小透明度
    [SerializeField] private float pakAlphaMax = 1.0f;  // 点滅最大透明度
    [SerializeField] private float pakFadeDuration = 0.4f;  // フェードアウト時間

    [Header("Button Group")]
    [SerializeField] private CanvasGroup buttonGroupCanvasGroup; // ボタン群をまとめた CanvasGroup
    [SerializeField] private float buttonGroupFadeInDuration = 0.5f;

    [Header("Buttons（上から順番に登録）")]
    [SerializeField] private List<Button> buttons = new List<Button>(); // Start / Credit / Quit の順

    [Header("Keyboard Selection Effect")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.18f, 0.95f, 0.95f, 1f); // シアン
    [SerializeField] private float colorFadeDuration = 0.12f;

    [Header("SE (任意)")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip buttonClickSE;
    [SerializeField] private AudioClip buttonHoverSE;
    [SerializeField] private AudioClip anyKeySE;

    // ─────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────

    private TitleManager titleManager;

    private enum TitlePhase { PressAnyKey, ButtonSelect }
    private TitlePhase phase = TitlePhase.PressAnyKey;

    private int selectedIndex = 0;   // 現在選択中のボタンインデックス
    private bool isTransitioning = false;
    private bool inputLocked = false; // フェード中などの入力ロック

    private Coroutine blinkCoroutine;
    private List<Coroutine> colorCoroutines = new List<Coroutine>();

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void Awake()
    {
        titleManager = GetComponent<TitleManager>();
        if (titleManager == null)
            Debug.LogError("[TitleUIController] TitleManager が見つかりません。同一GameObjectにアタッチしてください。");

        // 初期状態：ボタン群を非表示
        if (buttonGroupCanvasGroup != null)
        {
            buttonGroupCanvasGroup.alpha = 0f;
            buttonGroupCanvasGroup.interactable = false;
            buttonGroupCanvasGroup.blocksRaycasts = false;
        }

        // 初期状態：PressAnyKey を表示
        if (pressAnyKeyObject != null)
            pressAnyKeyObject.SetActive(true);
    }

    private void Start()
    {
        RegisterButtonMouseEvents();
        colorCoroutines = new List<Coroutine>(new Coroutine[buttons.Count]);

        // PressAnyKey の点滅開始
        blinkCoroutine = StartCoroutine(BlinkPressAnyKey());
    }

    private void Update()
    {
        if (isTransitioning || inputLocked) return;

        switch (phase)
        {
            case TitlePhase.PressAnyKey:
                HandlePressAnyKeyInput();
                break;

            case TitlePhase.ButtonSelect:
                HandleButtonSelectInput();
                break;
        }
    }

    // ─────────────────────────────────────────
    // Phase : PressAnyKey
    // ─────────────────────────────────────────

    private void HandlePressAnyKeyInput()
    {
        // 任意キー or マウスクリックで次のフェーズへ
        if (Input.anyKeyDown)
        {
            StartCoroutine(TransitionToButtonSelect());
        }
    }

    private IEnumerator TransitionToButtonSelect()
    {
        inputLocked = true;
        phase = TitlePhase.ButtonSelect;

        PlaySE(anyKeySE);

        // PressAnyKey の点滅を止めてフェードアウト
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        yield return StartCoroutine(FadeOutPressAnyKey());

        if (pressAnyKeyObject != null)
            pressAnyKeyObject.SetActive(false);

        // ボタン群をフェードイン
        yield return StartCoroutine(FadeInButtonGroup());

        // 最初のボタンを選択状態にする
        ApplySelectedColor(selectedIndex);

        inputLocked = false;
    }

    // ─────────────────────────────────────────
    // Phase : ButtonSelect（キーボード操作）
    // ─────────────────────────────────────────

    private void HandleButtonSelectInput()
    {
        int prev = selectedIndex;

        // 上移動：↑ or W
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = (selectedIndex + 1) % buttons.Count;
        }
        // 下移動：↓ or S
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = (selectedIndex - 1 + buttons.Count) % buttons.Count;
        }
        // 決定：Space or Enter
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteSelectedButton();
            return;
        }

        // 選択が変わった場合のみ色を更新
        if (selectedIndex != prev)
        {
            PlaySE(buttonHoverSE);
            TweenColor(prev, normalColor);
            ApplySelectedColor(selectedIndex);
        }
    }

    private void ExecuteSelectedButton()
    {
        if (selectedIndex < 0 || selectedIndex >= buttons.Count) return;
        PlaySE(buttonClickSE);
        buttons[selectedIndex].onClick.Invoke();
    }

    // ─────────────────────────────────────────
    // Button Click Callbacks
    // ─────────────────────────────────────────

    private void RegisterButtonMouseEvents()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // クロージャ用
            Button btn = buttons[i];
            if (btn == null) continue;

            // クリック
            btn.onClick.AddListener(() => OnButtonClicked(index));

            // マウスホバー
            var trigger = btn.gameObject.GetComponent<EventTrigger>()
                          ?? btn.gameObject.AddComponent<EventTrigger>();

            AddTriggerEntry(trigger, EventTriggerType.PointerEnter,
                _ => OnMouseEnter(index));
            AddTriggerEntry(trigger, EventTriggerType.PointerExit,
                _ => OnMouseExit(index));
        }
    }

    private void OnButtonClicked(int index)
    {
        if (!TryBeginTransition()) return;
        PlaySE(buttonClickSE);
        InvokeButtonAction(index);
    }

    private void OnMouseEnter(int index)
    {
        if (phase != TitlePhase.ButtonSelect || isTransitioning) return;
        if (selectedIndex != index)
        {
            TweenColor(selectedIndex, normalColor);
            selectedIndex = index;
        }
        PlaySE(buttonHoverSE);
        ApplySelectedColor(index);
    }

    private void OnMouseExit(int index)
    {
        if (phase != TitlePhase.ButtonSelect || isTransitioning) return;
        // マウスが外れても選択インデックスは維持し、色だけ少し暗くする
        // （キーボード操作と状態を合わせるため selectedIndex は変えない）
    }

    private void InvokeButtonAction(int index)
    {
        switch (index)
        {
            case 0: titleManager.OnStartGame(); break;
            case 1: titleManager.OnCredit(); break;
            case 2: titleManager.OnQuitGame(); break;
        }
    }

    // ─────────────────────────────────────────
    // Color / Selection
    // ─────────────────────────────────────────

    private void ApplySelectedColor(int index)
    {
        TweenColor(index, selectedColor);
    }

    private void TweenColor(int index, Color target)
    {
        if (index < 0 || index >= buttons.Count || buttons[index] == null) return;

        if (colorCoroutines[index] != null)
            StopCoroutine(colorCoroutines[index]);

        colorCoroutines[index] = StartCoroutine(TweenColorRoutine(buttons[index], target, colorFadeDuration));
    }

    private IEnumerator TweenColorRoutine(Button button, Color targetColor, float duration)
    {
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null) yield break;

        Color start = text.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(start, targetColor, elapsed / duration);
            yield return null;
        }
        text.color = targetColor;
    }

    // ─────────────────────────────────────────
    // Blink : Press Any Key
    // ─────────────────────────────────────────

    private IEnumerator BlinkPressAnyKey()
    {
        if (pressAnyKeyText == null) yield break;

        while (true)
        {
            float t = (Mathf.Sin(Time.time * pakBlinkSpeed * Mathf.PI) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(pakAlphaMin, pakAlphaMax, t);
            pressAnyKeyText.alpha = alpha;
            yield return null;
        }
    }

    private IEnumerator FadeOutPressAnyKey()
    {
        if (pressAnyKeyText == null) yield break;

        float start = pressAnyKeyText.alpha;
        float elapsed = 0f;

        while (elapsed < pakFadeDuration)
        {
            elapsed += Time.deltaTime;
            pressAnyKeyText.alpha = Mathf.Lerp(start, 0f, elapsed / pakFadeDuration);
            yield return null;
        }
        pressAnyKeyText.alpha = 0f;
    }

    // ─────────────────────────────────────────
    // Fade : Button Group
    // ─────────────────────────────────────────

    private IEnumerator FadeInButtonGroup()
    {
        if (buttonGroupCanvasGroup == null) yield break;

        buttonGroupCanvasGroup.interactable = false;
        buttonGroupCanvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < buttonGroupFadeInDuration)
        {
            elapsed += Time.deltaTime;
            buttonGroupCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / buttonGroupFadeInDuration);
            yield return null;
        }
        buttonGroupCanvasGroup.alpha = 1f;
        buttonGroupCanvasGroup.interactable = true;
        buttonGroupCanvasGroup.blocksRaycasts = true;
    }

    // ─────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────

    private bool TryBeginTransition()
    {
        if (isTransitioning) return false;
        isTransitioning = true;
        SetButtonsInteractable(false);
        return true;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in buttons)
            if (btn != null) btn.interactable = interactable;
    }

    private void AddTriggerEntry(EventTrigger trigger, EventTriggerType type,
                                  UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void PlaySE(AudioClip clip)
    {
        if (seSource != null && clip != null)
            seSource.PlayOneShot(clip);
    }
}