using UnityEngine;
using UnityEngine.UI;

public class KeywordButton : MonoBehaviour
{
    // --- この一行が足りない、または名前が間違っている可能性があります ---
    [SerializeField] private string _targetKeyword = "1";

    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        // データを保存
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.Unlock(_targetKeyword);
        }

        // WindowMemoが開いていたら、即座に反映させる
        if (WindowMemo.Instance != null)
        {
            // ここで _targetKeyword を使っているので、上で定義が必要です
            WindowMemo.Instance.ActivateContent(_targetKeyword);
        }

        Debug.Log($"{_targetKeyword} をクリックしました");
    }
}
