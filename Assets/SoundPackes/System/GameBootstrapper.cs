using UnityEngine;
using MyGame.AudioSetting;
public static class GameBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSystems()
    {
        //既に存在するか確認
        if (SoundService.Instance != null) return;

        //ResourcesファイルからSoundManagerを呼び出し
        var prefab = Resources.Load<GameObject>("SoundManagerPrefab");
        var instance = GameObject.Instantiate(prefab);

        //名前を調整
        instance.name = "SoundManager";
    }
}
