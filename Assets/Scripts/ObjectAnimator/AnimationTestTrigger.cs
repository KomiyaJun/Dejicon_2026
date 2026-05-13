using UnityEngine;

public class AnimationTestTrigger : MonoBehaviour
{
    public AnimationObserver observer;
    public float testValue = 2;

    [ContextMenu("Trigger Animation")]
    public void Test()
    {
        observer.TriggerScale(testValue);
    }
}