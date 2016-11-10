using System;
using UnityEngine;

public class Player : Character
{
    //DEBUG
    public enum TestType
    {
        Rune,
        Prayer,
        Ritual
    }

    public TestType testType;

    //

    private PlayerAttack selectedAttack;
    
    public override void Reset()
    {
        Stats = LoadData("player.caster");
        base.Reset();
    }

    public override void InitializeTurn()
    {
        // TODO: This is a temporary hardcoded attack. Should be selected through an UI of sorts.

        switch (testType)
        {
                case TestType.Prayer:
                {
                    selectedAttack = Instantiate(Resources.Load<GameObject>("Prefabs/pf_prayer")).GetComponent<PlayerAttack>();
                    selectedAttack.transform.parent = transform;
                    break;
            }
            case TestType.Rune:
                {        
                    selectedAttack = Instantiate(Resources.Load<GameObject>("Prefabs/pf_rune")).GetComponent<PlayerAttack>();
                    selectedAttack.transform.parent = transform;
                    break;
            }
            case TestType.Ritual:
                {
                    throw new ArgumentOutOfRangeException();
                }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }

    }

    public override void HandleTurn()
    {
        selectedAttack.HandleInput();
    }

    /// <summary>
    /// Attacks the enemy/enemies and destroys the rune object.
    /// </summary>
    /// <param name="info">The performed attack.</param>
    public override void PerformAttack(AttackInfo info)
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            DebugHelper.instance.AddMessage(string.Format("Player attacked for {0} damage", info.damage), 5);
            BattleManager.instance.AttackEnemies(info);
            Destroy(selectedAttack.gameObject);
            BattleManager.instance.DequeueCharacter();
        }, 2));
    }
}
