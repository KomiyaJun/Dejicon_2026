using UnityEngine;

namespace MyGame.AudioSetting
{
    [CreateAssetMenu(fileName = "NewLayeredSoundData", menuName = "Sound/LayeredBGM")]
    public class LayeredSoundData : ScriptableObject
    {
        [Header("Optional")]
        public AudioClip introMelody;
        public AudioClip introCords;
        public AudioClip introBass;
        public AudioClip introDrums;

        [Header("Core Layers")]
        public AudioClip melody;
        public AudioClip chords;
        public AudioClip bass;
        public AudioClip drums;

        [Header("Extra Layers")]
        public AudioClip[] extras;

        public float fadeDuration = 1.0f;

        //”z—ñ‚Æ‚µ‚Ä‘S‚Ä•Ô‚·
        public AudioClip[] GetAllClips()
        {
            int extraCount = (extras != null) ? extras.Length : 0;
            AudioClip[] all = new AudioClip[4 + extraCount];
            all[0] = melody;
            all[1] = chords;
            all[2] = bass;
            all[3] = drums;
            for (int i = 0; i < extraCount; i++)
            {
                all[i + 4] = extras[i];
            }
            return all;
        }
    }
}