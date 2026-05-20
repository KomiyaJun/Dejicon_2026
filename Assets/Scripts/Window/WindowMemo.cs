using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class WindowMemo : WindowBase
{
    // どこからでも WindowMemo.Instance でアクセスできるようにする
    public static WindowMemo Instance { get; private set; }

    [System.Serializable]
    public struct KeywordSetting
    {
        public string keyword;       // キーワード名（例: "Key1"）
        public GameObject targetObj; // 【修正！】GameObjectという名前から変更
    }

    [SerializeField] private List<KeywordSetting> _keywordSettings;

    protected override async UniTask OnOpen()
    {
        Instance = this;

        // アニメーション前に全オブジェクトを非表示にする
        foreach (var setting in _keywordSettings)
        {
            if (setting.targetObj != null)
                setting.targetObj.SetActive(false);
        }

        // アニメーション完了を待つ
        await base.OnOpen();

        // アニメーション後に解放済みのものだけ表示する
        if (GameDataManager.Instance != null)
        {
            foreach (var setting in _keywordSettings)
            {
                bool isUnlocked = GameDataManager.Instance.IsUnlocked(setting.keyword);
                if (setting.targetObj != null)
                    setting.targetObj.SetActive(isUnlocked);
            }
        }
    }

    // キーワードを活性化（表示）するメソッド
    public void ActivateContent(string key)
    {
        var setting = _keywordSettings.FirstOrDefault(s => s.keyword == key);

        if (setting.targetObj != null)
        {
            setting.targetObj.SetActive(true);

            // GameDataManager が存在する場合のみ保存する
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.Unlock(key);
                Debug.Log($"{key} を活性化しました");
            }
            else
            {
                Debug.LogWarning("GameDataManager.Instance が見つかりません");
            }
        }
        else
        {
            Debug.LogWarning($"{key} に対応するオブジェクトが見つかりません");
        }
    }

    protected override async UniTask OnClose()
    {
        await base.OnClose();
        if (Instance == this) Instance = null;
    }
}