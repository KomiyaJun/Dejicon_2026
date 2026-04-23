using UnityEngine;

namespace MyGame.AudioSetting
{
    public class SoundBridge : MonoBehaviour
    {
        //prefab且つDontDestroyOnLoadを利用しているSoundManagerに対する橋渡しを行うクラス

        [Header("フェード処理がある場合のFadeDuration")]
        [SerializeField] private float defaultFadeDuration = 1.0f;
        [Header("パート選択がある場合のパート")]
        [SerializeField] private BgmPartType part;
        public void PlaySE(SoundData data)
        {
            SoundService.Instance.PlaySE(data);
        }

        public void PlayBGM(SoundData data)
        {
            SoundService.Instance.PlayBGM(data, defaultFadeDuration);
        }

        public void PlayLayeredBGM(LayeredSoundData data)
        {
            SoundService.Instance.PlayLayeredBGM(data);
        }

        public void PlayLayeredBGMWithIntro(LayeredSoundData data)
        {
            SoundService.Instance.PlayLayeredBGMWithIntro(data);
        }

        public void PlayAmbient(SoundData data)
        {
            SoundService.Instance.PlayAmbient(data, defaultFadeDuration);
        }

        public void StopBGM()
        {
            SoundService.Instance.StopBGM(defaultFadeDuration);
        }

        public void StopAmbient()
        {
            SoundService.Instance.StopAmbient(defaultFadeDuration);
        }

        public void ActivateLayer(BgmPartType part) => SoundService.Instance.SetLayerVolume(part, defaultFadeDuration);
        public void DeactivateLayer(BgmPartType part) => SoundService.Instance.SetLayerVolume(part, defaultFadeDuration);

        public void SetLayerVolume(float volume)
        {
            SoundService.Instance.SetLayerVolume(part, volume, defaultFadeDuration);
        }

        public void SetEnviroment(string snapshotName)
        {
            SoundService.Instance.SetEnvironment(snapshotName, defaultFadeDuration);
        }

        public void SetAllLayersVolume(float volume)
        {
            SoundService.Instance.SetAllLayersVolume(volume, defaultFadeDuration);
        }

        public void ApplyPreset(BgmPreset preset)
        {
            SoundService.Instance.ApplyPreset(preset, defaultFadeDuration);
        }

        public void SetBgmPlaybackSpeed(float speed)
        {
            SoundService.Instance.SetBgmPlaybackSpeed(speed);
        }



        public void GetLayerSource(int index)
        {
            SoundService.Instance.GetLayerSource(index);
        }
        public void GetLayerSource(BgmPartType part)
        {
            SoundService.Instance.GetLayerSource(part);
        }

        public void GetBGMSource()
        {
            SoundService.Instance.GetBGMSource();
        }

    }
}