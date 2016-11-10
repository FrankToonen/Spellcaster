using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Prayer : PlayerAttack
{
    // TODO: What is this ritual going to be? 
    // TODO: Burn incense or some shit?

    protected override void Start()
    {
        base.Start();

        // Generate test set of points.
        drawnPoints = new List<Vector2>();
        points = new List<Vector2>() { new Vector2(0, 1), new Vector2(-1.5f, 0), new Vector2(1.5f, 0) };
        for (int i = 0; i < points.Count; i++)
        {
            var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/pf_testpoint"), points[i], Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            go.name = string.Format("testpoint {0}", i);
        }
    }

    /// <summary>
    /// Checks whether a touch touches a point in the collection.
    /// The points do not have to be drawn in a single swipe.
    /// If each point has been touched, the ritual has been completed.
    /// </summary>
    /// <param name="touch">The touch that has been detected.</param>
    public override void HandleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
            {
                CheckForPoints(Camera.main.ScreenToWorldPoint(touch.position));
                break;
            }
            case TouchPhase.Moved:
            {
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
                break;
            }
            case TouchPhase.Canceled:
            {
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// Checks if each point of the control points has been touched.
    /// </summary>
    /// <returns>Whether each point has been touched.</returns>
    protected override bool CheckSuccess()
    {
        if (drawnPoints.Count != points.Count)
        {
            return false;
        }

        return drawnPoints.Aggregate(true, (current, point) => current & points.Contains(point));
    }

    public override void Activate()
    {
        /*BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            AttackInfo info = new AttackInfo(AttackInfo.TargetType.Player, "Prayer", -10);

            DebugHelper.instance.AddMessage(string.Format("Player healed for {0} damage", info.damage), 5);
            BattleManager.instance.AttackTarget(info);
            Destroy(gameObject);
            BattleManager.instance.DequeueCharacter();
        }, 2));*/
    }
}
