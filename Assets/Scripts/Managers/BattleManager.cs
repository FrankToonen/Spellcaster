using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private Queue<BattleAction> battleActions;
    private float timeTillNextAction;

    private Queue<Character> battleOrder;
    private Character currentTurn;

    private Character player;
    private EnemyHorde enemies;

    private bool battleStarted;

    /// <summary>
    /// Creates a singleton of this Manager script.
    /// </summary>
    private void Awake()
    {
        instance = this;

        battleActions = new Queue<BattleAction>();
        battleOrder = new Queue<Character>();

        battleStarted = false;

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    /// <summary>
    /// Loads the enemies.
    /// </summary>
    public void LoadBattle( /*TODO: Pass the enemy parameters here*/)
    {
        enemies = new EnemyHorde();

        for (int i = -1; i < 2; i++)
        {
            // Instantiate basic enemy.
            GameObject e = Instantiate(Resources.Load<GameObject>("Prefabs/pf_enemy"));

            // Set its position and parent.
            var tempPosition = e.transform.position;
            tempPosition.x += i * 2;
            e.transform.position = tempPosition;
            e.transform.parent = transform;

            // Add it to the horde.
            enemies.AddEnemy(e.GetComponent<Enemy>());
        }

        battleStarted = true;

        DequeueCharacter();
    }

    public void EnqueueAction(BattleAction action)
    {
        battleActions.Enqueue(action);
    }

    public void DequeueCharacter()
    {
        if (battleOrder.Count <= 0)
        {
            EnqueueCharacters();
        }

        currentTurn = battleOrder.Dequeue();
        currentTurn.InitializeTurn();
    }

    private void EnqueueCharacters()
    {
        List<Character> characters = new List<Character>();
        characters.AddRange(enemies.enemies);
        characters.Add(player);

        characters = characters.OrderBy(c => c.Stats.Speed).Reverse().ToList();
        foreach (Character character in characters)
        {
            battleOrder.Enqueue(character);
        }
    }

    /// <summary>
    /// Fire the correct methods for the current TurnType.
    /// </summary>
    private void Update()
    {
        if (!battleStarted)
        {
            // DEBUG!
            if ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
            {
                LoadBattle();
            }
            //
            
            return;
        }

        if (battleActions.Count > 0 && Time.time >= timeTillNextAction)
        {
            var action = battleActions.Dequeue();
            action.method();

            timeTillNextAction = Time.time + action.duration;
        }
        
        if (currentTurn != null)
        {
            currentTurn.HandleTurn();
        }
        
        TryEndBattle();
        
        // DEBUG!
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameManager.SaveFile("player.caster", new CharacterData(100, 50,50,75));
            GameManager.SaveFile("enemy.caster", new CharacterData(100, 1, 50, 50));
        }
        //
    }

    /// <summary>
    /// Attacks all enemies.
    /// </summary>
    /// <param name="info">The attack to inflict on the enemies.</param>
    public void AttackEnemies(AttackInfo info)
    {
        enemies.AttackEnemies(info);
    }

    public void AttackPlayer(AttackInfo info)
    {
        player.ReceiveAttack(info);
    }

    private void TryEndBattle()
    {
        if (!enemies.IsDead && !player.IsDead)
        {
            return;
        }
        
        battleOrder.Clear();
        enemies.DestroyEnemies();

        string whoDied = enemies.IsDead ? "Enemy" : "Player";
        DebugHelper.instance.AddMessage(string.Format("{0} has died. The battle has ended", whoDied), 5);

        battleStarted = false;
    }
}
