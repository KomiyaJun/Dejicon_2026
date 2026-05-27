//ウィンドウからEDへのシーン遷移を行うスクリプト


using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTranslation_ed : MonoBehaviour
{
#if UNITY_EDITOR
    // エディタ上ではシーンアセットを直接参照する
    [SerializeField] private SceneAsset sceneAsset;
#endif

    [HideInInspector]
    [SerializeField] private string sceneName;

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("シーン名が設定されていません");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

#if UNITY_EDITOR
    // エディタ上でシーンアセットが変更されたときにシーン名を自動で取得する
    private void OnValidate()
    {
        if (sceneAsset != null)
            sceneName = sceneAsset.name;
        else
            sceneName = string.Empty;
    }
#endif
}