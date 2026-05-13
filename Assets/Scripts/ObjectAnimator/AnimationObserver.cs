using System;
using UnityEngine;

public class AnimationObserver : MonoBehaviour
{
    public event Action<int> OnScaleRequested;
    
    public void TriggerScale(int targetScale)
    {
        OnScaleRequested?.Invoke(targetScale);
    }
}
