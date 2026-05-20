using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    // 開放されたキーワードを保存するリスト
    private HashSet<string> _unlockedKeywords = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // キーワードを開放する
    public void Unlock(string key)
    {
        if (!_unlockedKeywords.Contains(key))
        {
            _unlockedKeywords.Add(key);
            Debug.Log($"{key} が開放されました");
        }
    }

    // すでに開放されているか確認する
    public bool IsUnlocked(string key)
    {
        return _unlockedKeywords.Contains(key);
    }
}