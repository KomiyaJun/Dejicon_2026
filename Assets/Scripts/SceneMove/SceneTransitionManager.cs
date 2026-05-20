using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private SceneTransitionController transitionController;

    // ★ 遷移先のシーンが「終わった瞬間」をキャッチするためのグローバルイベント
    public event Action OnTransitionComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 外部からこのメソッドを呼ぶだけでシーン遷移が始まります
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionSequence(sceneName));
    }

    private IEnumerator TransitionSequence(string sceneName)
    {
        // --- 1. FadeIn (画像が組み上がって画面が隠れるのを待つ) ---
        bool isFadeInComplete = false;
        Action handleFadeInEnd = null;
        handleFadeInEnd = () => {
            isFadeInComplete = true;
            transitionController.FadeInEnd -= handleFadeInEnd;
        };
        transitionController.FadeInEnd += handleFadeInEnd;

        transitionController.FadeIn();
        yield return new WaitUntil(() => isFadeInComplete);

        // --- 2. シーン切り替え ---
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // --- 3. FadeOut (画像がバラバラに消えて新しいシーンが見えるのを待つ) ---
        bool isFadeOutComplete = false;
        Action handleFadeOutEnd = null;
        handleFadeOutEnd = () => {
            isFadeOutComplete = true;
            transitionController.FadeOutEnd -= handleFadeOutEnd;
        };
        transitionController.FadeOutEnd += handleFadeOutEnd;

        transitionController.FadeOut();
        yield return new WaitUntil(() => isFadeOutComplete);

        // --- 4. 遷移先シーンへの通知 ---
        // フェードアウトが完全に終わったら、登録されているすべての処理を実行する
        Debug.Log("すべてのトランジションが終了しました。新シーンの処理を開始します。");
        OnTransitionComplete?.Invoke();
    }
}