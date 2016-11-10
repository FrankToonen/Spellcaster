using System;
using UnityEngine;

/// <summary>
/// An indirect extension class to the sealed 'Animation' class in UnityEngine.
/// This invokes an event when the animation has finished playing.
/// </summary>
[RequireComponent(typeof(Animation))]
public class AnimationExtension : MonoBehaviour
{
    public event Action OnAnimationFinished;

    private Animation animationComponent;
    private bool wasPlaying;

    private void Awake()
    {
        animationComponent = GetComponent<Animation>();
    }

    private void Update()
    {
        if (!animationComponent.isPlaying && wasPlaying)
        {
            if (OnAnimationFinished != null)
            {
                OnAnimationFinished.Invoke();
            }
        }

        wasPlaying = animationComponent.isPlaying;
    }
}