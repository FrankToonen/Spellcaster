using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Character
{
    /// <summary>
    /// TODO: AI decision making and shit.
    /// </summary>
    protected override void StartTurn()
    {
        // Don't make a decision if you can't attack.
        if (!canAttack)
        {
            BattleManager.instance.EnqueueAction(new BattleAction(BattleManager.instance.EndTurn, 0));
            return;
        }

        // find out if finding an attack is possible:
        BaseAttack attack;
        var possibleAttacks = attacks.All(a => a.CanBeUsed(this));
        if (possibleAttacks)
        {
            do
            {
                attack = attacks[Random.Range(0, attacks.Length)];

            } while (!attack.CanBeUsed(this));
        }
        else
        {
            BattleManager.instance.EndTurn();
            return;
        }

        // Find a target. "Single..." target attacks will pick an valid target at random, 
        // "None" target skills will target this character and "All..." skills will target every available target.
        var targets =
            BattleManager.instance.GetCharactersOfType(TargetToCharacterType(characterType, attack.info.targetType));
        var target = new List<Character> {this};
        if (targets.Count > 0)
        {
            if (attack.info.targetType == AttackInfo.TargetType.SingleEnemy ||
                attack.info.targetType == AttackInfo.TargetType.SingleFriendly)
            {
                target = new List<Character> {targets[Random.Range(0, targets.Count)]};
            }
            else
            {
                target = targets;
            }
        }

        PerformAttack(attack, target);
    }

    /// <summary>
    /// Destoys the enemy object.
    /// </summary>
    protected override void Die()
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            if (!IsDead)
            {
                return;
            }

            base.Die();

            DebugHelper.instance.AddMessage("Enemy killed");
            Destroy(gameObject);
        }, 0));
    }
}
