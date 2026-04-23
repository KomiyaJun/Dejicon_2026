using UnityEngine;
using MyGame.AudioSetting;

public class SceneMusicStarter : MonoBehaviour
{
    [Header("Options")]
    [Space(10)]
    [SerializeField] private float crossFadeTime = 2.0f;
    [Header("単一BGM再生[false] or レイヤー再生[true]")]
    [SerializeField] private bool isLayeredBGM = false;
    [Header("イントロ後に再生")]
    [SerializeField] private bool isIntro = false;

    [Header("単一のBGM再生")]
    [SerializeField] private SoundData sceneBGM;

    [Header("レイヤー分割されたBGM再生")]
    [SerializeField] private LayeredSoundData layeredBGM;

    [Header("環境音設定")]
    [SerializeField] private bool isAmbientON = false;
    [SerializeField] private SoundData ambientSound;

    void Start()
    {
        if (!isLayeredBGM)
        {
            //シーン開始時に再生リクエストを送信
            if (sceneBGM != null)
            {
                SoundService.Instance.PlayBGM(sceneBGM, crossFadeTime);
            }
        }
        else
        {
            if(layeredBGM != null)
            {
                if (!isIntro)
                {
                    SoundService.Instance.PlayLayeredBGM(layeredBGM);
                }
                else
                {
                    SoundService.Instance.PlayLayeredBGMWithIntro(layeredBGM);
                }
                    SoundService.Instance.SetAllLayersVolume(1);
            }
        }

        if (isAmbientON)
        {
            if (ambientSound == null) return;
            SoundService.Instance.PlayAmbient(ambientSound);
        }
    }


}
