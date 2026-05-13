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
        Instance = this; // インスタンスを登録
        await base.OnOpen();

        // 開いた時に、データマネージャーを確認して既に開放済みのものを表示する
        if (GameDataManager.Instance != null)
        {
            foreach (var setting in _keywordSettings)
            {
                bool isUnlocked = GameDataManager.Instance.IsUnlocked(setting.keyword);

                // 【修正！】targetObj を使用
                if (setting.targetObj != null)
                {
                    setting.targetObj.SetActive(isUnlocked);
                }
            }
        }
    }

    // キーワードを活性化（表示）するメソッド
    public void ActivateContent(string key)
    {
        // 該当するキーワード設定を探す
        var setting = _keywordSettings.FirstOrDefault(s => s.keyword == key);

        // 【修正！】targetObj を使用
        if (setting.targetObj != null)
        {
            setting.targetObj.SetActive(true); // オブジェクトを表示
            GameDataManager.Instance.Unlock(key); // データに保存
            Debug.Log($"{key} を活性化しました");
        }
    }

    protected override async UniTask OnClose()
    {
        await base.OnClose();
        if (Instance == this) Instance = null;
    }
}