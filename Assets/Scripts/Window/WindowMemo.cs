using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class WindowMemo : WindowBase
{
    public static WindowMemo Instance { get; private set; }

    [System.Serializable]
    public struct KeywordSetting
    {
        public string keyword;
        public GameObject targetObj;
    }

    [SerializeField] private List<KeywordSetting> _keywordSettings;

    [Header("全キーワード解放時に表示するボタン")]
    // 全キーワードが揃ったときに表示するボタン
    [SerializeField] private GameObject allUnlockedButton;

    protected override async UniTask OnOpen()
    {
        Instance = this;

        // アニメーション前に全オブジェクトを非表示にする
        foreach (var setting in _keywordSettings)
        {
            if (setting.targetObj != null)
                setting.targetObj.SetActive(false);
        }

        // ボタンも非表示にする
        if (allUnlockedButton != null)
            allUnlockedButton.SetActive(false);

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

        // 全キーワードが揃っているか確認する
        CheckAllUnlocked();
    }

    // キーワードを活性化するメソッド
    public void ActivateContent(string key)
    {
        var setting = _keywordSettings.FirstOrDefault(s => s.keyword == key);

        if (setting.targetObj != null)
        {
            setting.targetObj.SetActive(true);

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.Unlock(key);
                Debug.Log($"{key} を活性化しました");
            }
            else
            {
                Debug.LogWarning("GameDataManager.Instance が見つかりません");
            }

            // キーワードを活性化するたびに全キーワードが揃ったか確認する
            CheckAllUnlocked();
        }
        else
        {
            Debug.LogWarning($"{key} に対応するオブジェクトが見つかりません");
        }
    }

    // 全キーワードが揃ったか確認するメソッド
    private void CheckAllUnlocked()
    {
        if (GameDataManager.Instance == null) return;

        // 全キーワードが解放済みかどうか確認する
        bool allUnlocked = _keywordSettings.All(
            s => GameDataManager.Instance.IsUnlocked(s.keyword)
        );

        // 全キーワードが揃った場合はボタンを表示する
        if (allUnlocked && allUnlockedButton != null)
        {
            allUnlockedButton.SetActive(true);
            Debug.Log("全キーワードが揃いました");
        }
    }

    protected override async UniTask OnClose()
    {
        await base.OnClose();
        if (Instance == this) Instance = null;
    }
}