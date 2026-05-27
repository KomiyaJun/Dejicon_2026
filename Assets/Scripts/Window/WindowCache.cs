// WindowCache.cs
using UnityEngine;
using System.Collections.Generic;

public class WindowCache : MonoBehaviour
{
    // シングルトンでどこからでもアクセスできるようにする
    public static WindowCache Instance { get; private set; }

    // 共通キャッシュ
    private Dictionary<WindowData, WindowBase> cache
        = new Dictionary<WindowData, WindowBase>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ウィンドウを開くメソッド
    public void OpenWindow(WindowData data, Transform windowParent)
    {
        if (data == null)
        {
            Debug.LogWarning("WindowDataがアサインされていません");
            return;
        }

        if (windowParent == null)
        {
            Debug.LogWarning("Window_Parentが見つかりません");
            return;
        }

        // キャッシュに存在する場合は再利用する
        if (cache.TryGetValue(data, out WindowBase cachedWindow))
        {
            // すでに開いている場合は最前面に移動するだけ
            if (cachedWindow.gameObject.activeSelf)
            {
                cachedWindow.transform.SetAsLastSibling();
                return;
            }

            // 閉じている場合は開く
            cachedWindow.Open();
            return;
        }

        // キャッシュにない場合は新規生成する
        GameObject obj = Instantiate(data.prefab, windowParent);
        WindowBase window = obj.GetComponent<WindowBase>();

        if (window == null)
        {
            Debug.LogError($"{data.prefab.name}にWindowBaseがアタッチされていません");
            return;
        }

        window.SetUpWindow(data);
        cache[data] = window;
        window.Open();
    }

    // すでに開いているか確認するメソッド
    public bool IsOpen(WindowData data)
    {
        if (cache.TryGetValue(data, out WindowBase cachedWindow))
            return cachedWindow.gameObject.activeSelf;

        return false;
    }

    // キャッシュからウィンドウを取得するメソッド
    public WindowBase GetWindow(WindowData data)
    {
        if (cache.TryGetValue(data, out WindowBase cachedWindow))
            return cachedWindow;

        return null;
    }
}