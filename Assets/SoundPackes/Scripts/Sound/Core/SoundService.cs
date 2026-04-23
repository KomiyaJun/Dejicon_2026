using UnityEngine;

namespace MyGame.AudioSetting
{
    public class SoundService
    {
        public static ISoundManager Instance { get; private set; }

        public static void Provide(ISoundManager manager)
        {
            Instance = manager;
        }
    }
}
