using UnityEngine;

public class AnimationTestTrigger : MonoBehaviour
{
    public AnimationObserver observer;
    public int testValue = 2;

    [ContextMenu("Trigger Animation")]
    public void Test()
    {
        observer.TriggerScale(testValue);
    }
}