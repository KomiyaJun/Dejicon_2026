using MyGame.AudioSetting;
using Unity.VisualScripting;
using UnityEngine;

public class EpisodeManager : MonoBehaviour
{
    // どんなシーンのイニシャライザでも、これ1つで受け止められる
    [SerializeField] private BaseSceneInitiator sceneInitiator;

    [SerializeField] private bool isSoundPlay = false;
    [SerializeField] private SoundData bgmData;

    private void Start()
    {
        // 1. 裏側の準備（中身が何かは知らないが、とにかく準備を命令する）
        if (sceneInitiator != null)
        {
            sceneInitiator.PrepareSceneData();
        }

        // 2. トランジションの終了を待つ
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.OnTransitionComplete += StartEpisode;

            if (isSoundPlay)
            {
                SoundService.Instance.PlayBGM(bgmData);
            }
        }
        else
        {
            if (isSoundPlay)
            {
                SoundService.Instance.PlayBGM(bgmData);
            }

            StartEpisode();
        }
    }

    private void StartEpisode()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.OnTransitionComplete -= StartEpisode;
        }

        Debug.Log("画面のフェードアウトが完了。シーンの本番処理を開始します。");

        // 3. 本番開始（中身が何かは知らないが、とにかく開始を命令する）
        if (sceneInitiator != null)
        {
            sceneInitiator.StartScene();
        }
    }
}