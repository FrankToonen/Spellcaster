using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A collection of enemies to make the code in the BattleManager a bit cleaner.
/// This handles looping over all the enemies in the battle.
/// </summary>
public class EnemyHorde
{
    public readonly List<Character> enemies;

    public EnemyHorde()
    {
        enemies = new List<Character>();
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void DestroyEnemies()
    {
        foreach (Character enemy in enemies)
        {
            GameObject.Destroy(enemy.gameObject);
        }
    }

    public void InitilizeTurn()
    {
        foreach (ICharacter enemy in enemies)
        {
            enemy.InitializeTurn();
        }
    }

    public void AttackEnemies(AttackInfo info)
    {
        foreach (ICharacter enemy in enemies)
        {
            enemy.ReceiveAttack(info);
        }
    }

    public bool IsDead
    {
        get
        {
            return enemies.Aggregate(true, (current, enemy) => current & enemy.IsDead);
        }
    }
}
