using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    
    [Tooltip("The scale if the curve evaluates to 1.")]
    [SerializeField] private float scaleScaler = 5;
    
    private Camera cameraComponent;

    /// <summary>
    /// Creates a singleton instance of this class and caches the camera component.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        cameraComponent = GetComponent<Camera>();
    }

    /// <summary>
    /// Plays an animation that moves the camera along a bezier curve and scales it using an AnimationCurve.
    /// This will start a coroutine that ends after the CameraAnimationInfo's duration has passed.
    /// </summary>
    /// <param name="animationInfo">The information about the animation.</param>
    /// <param name="loop">Whether the animation should loop or not.</param>
    public void PlayCameraAnimation(CameraAnimationInfo animationInfo, bool loop = false)
    {
        if (animationInfo.scaleCurve.keys.Length == 0)
        {
            throw new Exception("CameraController: Scale curve is invalid");
        }

        StopAllCoroutines();
        StartCoroutine(PlayAnimation(animationInfo, loop));
    }
    
    private IEnumerator PlayAnimation(CameraAnimationInfo animationInfo, bool loop)
    {
        var startTime = Time.time;
        var endTime = Time.time + animationInfo.duration;
        
        while (Time.time < endTime || loop)
        {
            // The interval should be a number between 0 and 1.
            var interval = ((Time.time - startTime) / animationInfo.duration) % 1;

            // Set the scale and position at the given interval.
            // Set the z-position to a constant value to prevent it from getting behind objects.
            transform.position = DeCasteljau(interval, animationInfo.controlPoints);
            transform.position += new Vector3(0,0,-10);
            cameraComponent.orthographicSize = animationInfo.scaleCurve.Evaluate(interval) * scaleScaler;
            
            yield return null;
        }

        // Reset the position and scale.
        transform.position = new Vector3(0,0,1) * transform.position.z;
        cameraComponent.orthographicSize = scaleScaler;
    }

    /// <summary>
    /// Finds the position on the curve for a given t using the De Casteljau algorithm.
    /// Has a complexity of O(n^2) where n is the number of control points for the curve.
    /// </summary>
    /// <param name="t">The t parameter that will be mapped to a point on the curve.</param>
    /// <param name="points">The control points to use.</param>
    /// <returns>The position.</returns>
    private static Vector2 DeCasteljau(float t, IList<Vector2> points)
    {
        if (points.Count == 0)
        {
            throw new ArgumentException();
        }

        if (points.Count == 1)
        {
            return points[0];
        }

        // Point count >= 2
        var newPoints = new List<Vector2>();
        for (var i = 0; i < points.Count - 1; i++)
        {
            newPoints.Add(points[i] + (points[i + 1] - points[i]) * t);
        }

        return DeCasteljau(t, newPoints);
    }
}

/// <summary>
/// Information about a camera animation. This will move the camera between the control points as a bezier curve. 
/// Simultaniously it will take its new scale from the AnimationCurve.
/// The duration dictates how long the animation will take in seconds.
/// </summary>
[Serializable]
public struct CameraAnimationInfo
{
    [Tooltip("Along which points the bezier curve will move.")]
    public List<Vector2> controlPoints;

    [Tooltip("How long the animation will take in seconds.")]
    public int duration;

    [Tooltip("The curve of the scale the camera will follow.")]
    public AnimationCurve scaleCurve;

    public CameraAnimationInfo(List<Vector2> controlPoints, int duration, AnimationCurve scaleCurve)
    {
        this.controlPoints = controlPoints;
        this.duration = duration;
        this.scaleCurve = scaleCurve;
    }
}
