using System;
using System.Collections.Generic;
using UnityEngine;

public class Rune : PlayerAttack
{
    protected override void Start()
    {
        base.Start();

        // Generate test set of points.
        drawnPoints = new List<Vector2>();
        points = new List<Vector2>() { new Vector2(0, 1), new Vector2(-1.5f, 0), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1.5f, 0) };
        for (int i = 0; i < points.Count; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/pf_testpoint"), points[i], Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            go.name = string.Format("testpoint {0}", i);
        }
    }

    public override void HandleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
            {
                break;
            }
            case TouchPhase.Moved:
            {
                CheckForPoints(Camera.main.ScreenToWorldPoint(touch.position));
                break;
            }
            case TouchPhase.Stationary:
            {
                break;
            }
            case TouchPhase.Ended:
            {
                if (CheckSuccess())
                {
                    Activate();
                }
                else
                {
                    ResetDrawing();
                }
                break;
            }
            case TouchPhase.Canceled:
            {
                ResetDrawing();
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// Checks whether each drawn point matches the control points per index.
    /// </summary>
    /// <returns>Whether the collections are an exact match.</returns>
    protected override bool CheckSuccess()
    {
        if (drawnPoints.Count != points.Count)
        {
            return false;
        }

        bool success = true;
        for (int i = 0; i < points.Count; i++)
        {
            success &= points[i] == drawnPoints[i];
        }
        return success;
    }

    /// <summary>
    /// Activates the rune after a correct pattern was drawn.
    /// </summary>
    public override void Activate()
    {
        //AttackInfo attack = new AttackInfo(AttackInfo.TargetType.Enemy, "Rune", 50);
        //player.PerformAttack(attack);
    }
}
