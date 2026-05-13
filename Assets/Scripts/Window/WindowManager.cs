using UnityEngine;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour, IWindowManager
{
    [SerializeField] private Transform windowParent;

    private Dictionary<WindowData, WindowBase> instantiatedWindows = new Dictionary<WindowData, WindowBase>();

    private void Awake()
    {
        WindowService.Provide(this);
    }

    // 追加：指定されたデータのウィンドウが存在し、かつアクティブ（表示中）かを返す
    public bool IsWindowVisible(WindowData data)
    {
        if (data == null) return false;
        return instantiatedWindows.TryGetValue(data, out var window) && window.gameObject.activeSelf;
    }

    public void OpenWindow(WindowData data)
    {
        if (data == null) return;

        if (!instantiatedWindows.TryGetValue(data, out var window))
        {
            GameObject obj = Instantiate(data.prefab, windowParent);
            window = obj.GetComponent<WindowBase>();
            window.SetUpWindow(data);
            instantiatedWindows.Add(data, window);
        }

        window.Open();
        window.transform.SetAsLastSibling();
    }

    public void CloseWindow(WindowData data)
    {
        if (instantiatedWindows.TryGetValue(data, out var window))
        {
            window.Close();
        }
    }
}