using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using MyGame.AudioSetting;

public class EpilogueTransitionController : MonoBehaviour, IPointerClickHandler
{
    [Header("表示するセリフ")]
    [TextArea(2, 5)]
    public string dialogText = "すべての真実を知る覚悟はありますか？";

    [Header("UI参照")]
    public TextMeshProUGUI dialogTextUI;
    public TextMeshProUGUI clickPromptUI;

    [Header("テキストのフェード速度")]
    public float fadeSpeed = 2f;

    [Header("遷移先のシーン名")]
    public string nextSceneName = "Epilogue";

    private bool isWaitingForClick = false;

    // プレハブのボタンからでも呼べるようにシングルトン化
    public static EpilogueTransitionController Instance { get; private set; }

    [Header("音関連")]
    [SerializeField] private SoundData ClickData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 初期状態では自身（UIパネル全体）を非表示にする
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // クリック待ち状態の時だけ「クリックで次へ」テキストを点滅させる
        if (isWaitingForClick && clickPromptUI != null)
        {
            Color c = clickPromptUI.color;
            // PingPongでアルファ値を反復。下限を0.2にして完全に消えないようにする
            c.a = Mathf.PingPong(Time.time * fadeSpeed, 0.8f) + 0.2f;
            clickPromptUI.color = c;
        }
    }

    // ★ 外部のボタンから呼び出されるメソッド
    public void StartTransition()
    {
        gameObject.SetActive(true);
        if (dialogTextUI != null)
        {
            dialogTextUI.text = dialogText;
        }
        
        if (clickPromptUI != null)
        {
            clickPromptUI.gameObject.SetActive(true);
        }

        isWaitingForClick = true;
    }

    // パネル全体がクリックされた時の処理
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isWaitingForClick)
        {
            isWaitingForClick = false; // 連打防止

            PlaySE(ClickData);
            SoundService.Instance.StopBGM();
            // SceneTransitionManager が存在すればそれを使ってトランジション遷移
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(nextSceneName);
            }
            else
            {
                // 無ければ即時ロード
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void PlaySE(SoundData data)
    {
        SoundService.Instance.PlaySE(data);
    }
}
