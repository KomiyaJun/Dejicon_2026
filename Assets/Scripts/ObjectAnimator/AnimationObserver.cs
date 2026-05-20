using System;
using UnityEngine;

public class AnimationObserver : MonoBehaviour
{
    public event Action<float> OnScaleRequested;
    
    public void TriggerScale(float targetScale)
    {
        OnScaleRequested?.Invoke(targetScale);
    }
}
