using UnityEngine;
using System.Collections;
using System;

public class SceneTransitionController : MonoBehaviour
{
    // [SerializeField] は削除し、外部から登録できるようにする
    public event Action FadeOutEnd;
    public event Action FadeInEnd;

    public Material transitionMaterial;
    public float duration = 1.5f;

    private Coroutine _currentTransition;

    [ContextMenu("Fade Out (0 -> 1)")]
    public void FadeOut()
    {
        // 終了時に呼び出したいイベント（FadeOutEnd）を一緒に渡す
        Play(0, 1, FadeOutEnd);
    }

    [ContextMenu("Fade In (1 -> 0)")]
    public void FadeIn()
    {
        // 終了時に呼び出したいイベント（FadeInEnd）を一緒に渡す
        Play(1, 0, FadeInEnd);
    }

    // 引数に Action を追加
    private void Play(float start, float end, Action onCompleteEvent)
    {
        if (_currentTransition != null) StopCoroutine(_currentTransition);
        _currentTransition = StartCoroutine(AnimateTransition(start, end, onCompleteEvent));
    }

    // 引数に Action を追加
    IEnumerator AnimateTransition(float start, float end, Action onCompleteEvent)
    {
        Debug.Log($"Transition Started: {start} to {end}");

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            float progress = Mathf.Lerp(start, end, normalizedTime);

            transitionMaterial.SetFloat("_Progress", progress);
            yield return null;
        }

        transitionMaterial.SetFloat("_Progress", end);
        _currentTransition = null;
        Debug.Log("Transition Finished!");

        // イベントが登録されていれば実行する（?. は安全に呼び出すため）
        onCompleteEvent?.Invoke();
    }
}