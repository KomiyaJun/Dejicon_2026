using UnityEngine;

public class WindowBridge : MonoBehaviour
{
    [SerializeField] private WindowData data;

    public void OnButtonClicked()
    {
        if (WindowService.Instance == null || data == null) return;

        // 表示中なら閉じ、非表示（または未生成）なら開く
        if (WindowService.Instance.IsWindowVisible(data))
        {
            WindowService.Instance.CloseWindow(data);
        }
        else
        {
            WindowService.Instance.OpenWindow(data);
        }
    }
}