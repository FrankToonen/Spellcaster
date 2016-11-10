using UnityEngine;
using System.Collections.Generic;

public abstract class PlayerAttack : MonoBehaviour
{
    protected Player player;

    protected List<Vector2> points;
    protected List<Vector2> drawnPoints;
    
    protected float detectionRadius;

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        
        detectionRadius = 1;
    }

    /// <summary>
    /// Handles the input for the object this script is attached to.
    /// This looks at the first touch and uses its TouchPhase to determine appropriate action.
    /// </summary>
    public void HandleInput()
    {
        // DEBUG: PC controls
        if (Input.GetMouseButtonDown(0) && Input.touchCount == 0)
        {
            Activate();
        }
        //

        if (Input.touchCount <= 0)
        {
            return;
        }

        HandleTouch(Input.GetTouch(0));
    }

    /// <summary>
    /// Handles a touch that has been detected.
    /// </summary>
    /// <param name="touch"></param>
    public abstract void HandleTouch(Touch touch);

    /// <summary>
    /// Checks for points in the points collection to see if they were touched.
    /// </summary>
    /// <param name="location">The location of the touch in world coordinates.</param>
    protected void CheckForPoints(Vector2 location)
    {
        foreach (Vector2 point in points)
        {
            if (Vector2.Distance(location, point) < detectionRadius && !drawnPoints.Contains(point))
            {
                drawnPoints.Add(point);
            }
        }
    }
    
    /// <summary>
    /// Resets drawing. Clears the drawn points and sets the booleans to the correct values to enable restarting drawing.
    /// </summary>
    protected void ResetDrawing()
    {
        drawnPoints.Clear();
    }

    /// <summary>
    /// Checks whether the drawn pattern was the correct one.
    /// This is done by comparing the drawn points to the actual points.
    /// The process differs per type of attack.
    /// </summary>
    /// <returns>Whether the pattern was correct or not.</returns>
    protected abstract bool CheckSuccess();

    /// <summary>
    /// Activates the attack after a successful ritual.
    /// </summary>
    public abstract void Activate();
}
