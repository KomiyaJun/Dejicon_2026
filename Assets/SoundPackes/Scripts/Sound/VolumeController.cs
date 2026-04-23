using UnityEngine;
using UnityEngine.UI;
using MyGame.AudioSetting;

public class VolumeController : MonoBehaviour
{
    [Header("メインボリューム")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider ambientSlider;

    [Header("パート別ボリューム")]
    [SerializeField] private Slider melodySlider;
    [SerializeField] private Slider chordsSlider;
    [SerializeField] private Slider bassSlider;
    [SerializeField] private Slider drumSlider;

    [Header("参照")]
    [SerializeField] SoundBridge _soundBridge;


    private const string KeyMasterVolume = "MasterVolume";
    private const string KeyBGMVolume = "BGMVolume";
    private const string KeySEVolume = "SEVolume";
    private const string KeyUIVolume = "UIVolume";
    private const string KeyAmbientVolume = "AmbientVolume";

    private const string KeyMelody = "Part_Melody";
    private const string KeyChords = "Part_Chords";
    private const string KeyBass = "Part_Bass";
    private const string KeyDrum = "Part_Drum";

    private BgmPreset runtimeUserPreset;

    private void Start()
    {
        runtimeUserPreset = ScriptableObject.CreateInstance<BgmPreset>();

        //起動時に保存されている値をスライダーに反映する
        SetCurrentParamet();

        
        //スライダー動作時のイベント登録
        masterSlider.onValueChanged.AddListener(v => SetGlobalVol(VolumeType.Master, v, KeyMasterVolume));
        bgmSlider.onValueChanged.AddListener(v => SetGlobalVol(VolumeType.BGM, v, KeyBGMVolume));

        melodySlider.onValueChanged.AddListener(v => UpdatePartVolume(KeyMelody, v));
        chordsSlider.onValueChanged.AddListener(v => UpdatePartVolume(KeyChords, v));
        bassSlider.onValueChanged.AddListener(v => UpdatePartVolume(KeyBass, v));
        drumSlider.onValueChanged.AddListener(v => UpdatePartVolume(KeyDrum, v));

        ApplyCurrentPartSettings();


        ////デモ版では不要なため除外
        
        //seSlider.value = PlayerPrefs.GetFloat(KeySEVolume, 1.0f);
        //uiSlider.value = PlayerPrefs.GetFloat(KeyUIVolume, 1.0f);
        //ambientSlider.value = PlayerPrefs.GetFloat(KeyAmbientVolume, 1.0f);

        //seSlider.onValueChanged.AddListener(v => SetVol(VolumeType.SE, v));
        //uiSlider.onValueChanged.AddListener(v => SetVol(VolumeType.UI, v));
        //ambientSlider.onValueChanged.AddListener(v => SetVol(VolumeType.Ambient, v));
    }

    private void SetGlobalVol(VolumeType type, float value, string key)
    {
        SoundService.Instance.SetVolume(type, value);
        PlayerPrefs.SetFloat(key, value);
    }

    private void UpdatePartVolume(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        ApplyCurrentPartSettings();
    }

    private void ApplyCurrentPartSettings()
    {
        runtimeUserPreset.melodyVolume = melodySlider.value;
        runtimeUserPreset.chordsVolume = chordsSlider.value;
        runtimeUserPreset.bassVolume = bassSlider.value;
        runtimeUserPreset.drumVolume = drumSlider.value;

        _soundBridge.ApplyPreset(runtimeUserPreset);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }

    public void SetCurrentParamet()
    {
        masterSlider.value = PlayerPrefs.GetFloat(KeyMasterVolume, 1.0f);
        bgmSlider.value = PlayerPrefs.GetFloat(KeyBGMVolume, 1.0f);

        melodySlider.value = PlayerPrefs.GetFloat(KeyMelody, 1.0f);
        chordsSlider.value = PlayerPrefs.GetFloat(KeyChords, 1.0f);
        bassSlider.value = PlayerPrefs.GetFloat(KeyBass, 1.0f);
        drumSlider.value = PlayerPrefs.GetFloat(KeyDrum, 1.0f);
    }
}
