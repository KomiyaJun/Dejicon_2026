using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShowMapKoukuButton : MonoBehaviour
{
    [Header("このボタン")]
    [SerializeField] private Button button;

    [Header("押すたびに表示・非表示を切り替える")]
    [SerializeField] private bool toggleMode = true;

    [Header("同じボタンでMapWindowを開く場合はON")]
    [SerializeField] private bool waitUntilMapWindowAppears = true;

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        if (waitUntilMapWindowAppears)
        {
            StartCoroutine(ExecuteAfterMapWindowAppears());
        }
        else
        {
            Execute();
        }
    }

    private IEnumerator ExecuteAfterMapWindowAppears()
    {
        float time = 0f;
        float timeout = 0.5f;

        while (MapKoukuController.ActiveInstance == null && time < timeout)
        {
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        Execute();
    }

    private void Execute()
    {
        MapKoukuController controller = MapKoukuController.ActiveInstance;

        if (controller == null)
        {
            controller = FindFirstObjectByType<MapKoukuController>();
        }

        if (controller == null)
        {
            Debug.LogWarning("Scene上に MapKoukuController が見つかりません。MapWindowが開いているか確認してください。");
            return;
        }

        if (toggleMode)
        {
            controller.ToggleKouku();
        }
        else
        {
            controller.ShowKoukuOn();
        }
    }
}