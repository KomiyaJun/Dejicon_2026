using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using MyGame.AudioSetting;

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


    [Header("音関連")]
    [SerializeField] private SoundData pencilData;

    protected override async UniTask OnOpen()
    {
        Instance = this;

        // アニメーション前に全オブジェクトを非表示にする
        foreach (var setting in _keywordSettings)
        {
            if (setting.targetObj != null)
                setting.targetObj.SetActive(false);
        }

        if (allUnlockedButton != null)
            allUnlockedButton.SetActive(false);

        await base.OnOpen();

        // アニメーション後に解放済みのものだけ表示する
        if (GameDataManager.Instance != null)
        {
            foreach (var setting in _keywordSettings)
            {
                bool isUnlocked = GameDataManager.Instance.IsUnlocked(setting.keyword);
                if (setting.targetObj != null)
                {
                    setting.targetObj.SetActive(isUnlocked);

                    // 【追記】すでに解放済みのものは、演出なしで文字を表示状態(Alpha=1)にする
                    if (isUnlocked && setting.targetObj.TryGetComponent<MemoEffect>(out var effect))
                    {
                        effect.SetUnlockedState();
                    }
                }
            }
        }

        CheckAllUnlocked();
    }
    // キーワードを活性化するメソッド
    public void ActivateContent(string key)
    {
        var setting = _keywordSettings.FirstOrDefault(s => s.keyword == key);

        if (setting.targetObj != null)
        {
            // 【変更】ただSetActive(true)するのではなく、演出コンポーネントがあれば再生する
            if (setting.targetObj.TryGetComponent<MemoEffect>(out var effect))
            {
                // Forget() を使うことで、このメソッド自体の処理の流れ（SE再生や解放データ更新）を止めずに
                // バックグラウンドで非同期に演出アニメーションを実行できます。
                effect.PlayAnimationAsync(destroyCancellationToken).Forget();
            }
            else
            {
                // 万が一コンポーネントがついてない場合は、従来通りの挙動にする
                setting.targetObj.SetActive(true);
            }

            // --- 以下の処理内容は一切変更していません ---
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.Unlock(key);
                Debug.Log($"{key} を活性化しました");
                PlaySE(pencilData);
            }
            else
            {
                Debug.LogWarning("GameDataManager.Instance が見つかりません");
            }

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

    private void PlaySE(SoundData data)
    {
        SoundService.Instance.PlaySE(data);
    }
}