// WindowCache.cs
using UnityEngine;
using System.Collections.Generic;

public class WindowCache : MonoBehaviour
{
    public static WindowCache Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OpenWindow(WindowData data, Transform windowParent)
    {
        if (WindowService.Instance != null)
        {
            WindowService.Instance.OpenWindow(data);
        }
    }

    public bool IsOpen(WindowData data)
    {
        if (WindowService.Instance != null)
        {
            return WindowService.Instance.IsWindowVisible(data);
        }
        return false;
    }

    public WindowBase GetWindow(WindowData data)
    {
        if (WindowService.Instance != null)
        {
            return WindowService.Instance.GetWindow(data);
        }
        return null;
    }
}