using UnityEngine;

// WindowManagerが実装すべきインターフェース
public interface IWindowManager
{
    void OpenWindow(WindowData data);
    void CloseWindow(WindowData data);
    // 追加：ウィンドウが表示中かどうかを判定する
    bool IsWindowVisible(WindowData data);
}

// ウィンドウシステム用のサービスロケータ
public static class WindowService
{
    public static IWindowManager Instance { get; private set; }

    public static void Provide(IWindowManager manager)
    {
        Instance = manager;
    }
}