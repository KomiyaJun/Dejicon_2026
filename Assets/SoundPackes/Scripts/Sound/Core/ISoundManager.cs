using UnityEngine;

namespace MyGame.AudioSetting
{
    public interface ISoundManager
    {
        //SE再生
        void PlaySE(SoundData data);

        //BGM再生
        void PlayBGM(SoundData data, float fadeDuration = 1.0f);

        //レイヤーBGMの再生
        void PlayLayeredBGM(LayeredSoundData data);

        //イントロ後にループBGMを再生
        void PlayLayeredBGMWithIntro(LayeredSoundData data);

        //環境音の再生
        void PlayAmbient(SoundData data, float fadeDuration = 1.0f);

        //BGM停止
        void StopBGM(float fadeDuration = 1.0f);

        //環境音の停止
        void StopAmbient(float fadeDuration = 1.0f);

        //音量変更
        void SetVolume(VolumeType type, float volume);

        //レイヤーのindexに該当する部分の音量を調整
        void SetLayerVolume(BgmPartType part, float volume, float duration = 0.0f);
        void SetLayerVolume(int index, float volume, float duration);

        //環境によってのスナップショット変化
        void SetEnvironment(string snapShotName, float duration = 1.0f);

        //レイヤーの全ての音量を均一で変更
        void SetAllLayersVolume(float volume, float duration = 0.5f);

        //音量プリセットを使って音量を調節
        void ApplyPreset(BgmPreset preset, float duratio = 1.0f);

        void SetBgmPlaybackSpeed(float speed);

        //指定レイヤーのAudioSourceを取得
        AudioSource GetLayerSource(int index);
        AudioSource GetLayerSource(BgmPartType part);

        //通常のBGMのAudioSorceを取得
        AudioSource GetBGMSource();

    }
}
