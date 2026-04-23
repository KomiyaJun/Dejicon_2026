using UnityEngine;

namespace MyGame.AudioSetting
{
    [CreateAssetMenu(fileName = "NewBgmPreset", menuName = "Sound/BgmPreset")]
    public class BgmPreset : ScriptableObject
    {
        [Range(0f, 1f)] public float melodyVolume = 1f;
        [Range(0f, 1f)] public float chordsVolume = 1f;
        [Range(0f, 1f)] public float bassVolume = 1f;
        [Range(0f, 1f)] public float drumVolume = 1f;
        [Range(0f, 1f)] public float[] extraVolumes;

        public float[] GetVolumeArray()
        {
            int extraCount = (extraVolumes != null) ? extraVolumes.Length : 0;
            float[] v = new float[4 + extraCount];
            v[0] = melodyVolume;
            v[1] = chordsVolume;
            v[2] = bassVolume;
            v[3] = drumVolume;
            for (int i = 0; i < extraCount; i++)
            {
                v[i + 4] = extraVolumes[i];
            }
            return v;
        }


    }
}