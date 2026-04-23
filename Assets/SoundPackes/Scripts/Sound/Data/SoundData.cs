using UnityEngine;
using UnityEngine.Audio;

namespace MyGame.AudioSetting
{
    [CreateAssetMenu(fileName = "newSoundData", menuName = "Sound/SoundData")]
    public class SoundData : ScriptableObject
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        [Range(0f, 1f)] public float volume;
        [Range(0f, 1f)] public float pitch;
        public bool loop;

        public bool useRandomPitch;
        [Range(0, 0.5f)] public float pitchRandomRange = 0.1f;
    }
}