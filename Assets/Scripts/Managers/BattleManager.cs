using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Singleton reference to this class.
    public static BattleManager instance;

    // DEBUG VARIABLES!
    [SerializeField] private int amountOfEnemies = 1;
    [SerializeField] private bool debugSpeed = false;
    //

    // The queue of actions that should be invoked in order and after their duration has passed.
    private Queue<BattleAction> battleActions;
    private float timeTillNextAction;

    // All the Characters engaged in battle.
    private List<Character> characters;

    // The Character whose turn it is. This is used to find whose turn it is next.
    private Character currentTurnCharacter;

    // Where Characters can spawn.
    [SerializeField] private List<SpawnPoint> spawnPoints;

    // Whether the battle has started.
    private bool battleStarted;

    /// <summary>
    /// Creates a singleton of this Manager script.
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

        battleActions = new Queue<BattleAction>();
        battleStarted = false;
    }

    // DEBUG
    private void Start()
    {
        GlobalMessage.instance.BroadCastMessage("Press 'Space' to start a battle", -1);
    } 
    //

    /// <summary>
    /// Loads the enemies.
    /// </summary>
    public void LoadBattle( /*TODO: Pass the enemy parameters here*/)
    {
        // TODO: Add friendly characters differently.
        characters = new List<Character>();

        AddCharacterToBattle(GameObject.FindWithTag("Player").GetComponent<Character>());

        for (var i = 0; i < amountOfEnemies; i++)
        {
            // Instantiate basic enemy.
            var enemyObject = Instantiate(Resources.Load<GameObject>("Prefabs/pf_enemy"), transform) as GameObject;
            enemyObject.name = "Enemy " + i;

            // Add it to the horde.
            AddCharacterToBattle(enemyObject.GetComponent<Character>());
        }

        battleStarted = true;
        StartNextTurn();
    }

    /// <summary>
    /// Finds the first available SpawnPoint for the passed CharacterType. 
    /// </summary>
    /// <param name="type">The CharacterType to find a SpawnPoint for.</param>
    /// <returns>The first available SpawnPoint.</returns>
    private SpawnPoint GetSpawnPoint(Character.CharacterType type)
    {
        if (!CanSpawn(type))
        {
            return null;
        }
        
        return spawnPoints.First(s => !s.taken && s.availableToCharactersOfType == type);
    }

    /// <summary>
    /// Checks whether a spawnpoint is available.
    /// </summary>
    /// <param name="type">Which type of spawnpoint to check for.</param>
    /// <returns>If there is a spawnpoint available.</returns>
    public bool CanSpawn(Character.CharacterType type)
    {
        return spawnPoints.FindAll(s => s.availableToCharactersOfType == type && !s.taken).Count > 0;
    }

    /// <summary>
    /// Adds a character to the battle. The list is then sorted on character speed.
    /// </summary>
    /// <param name="character">The character to add to the battle.</param>
    public void AddCharacterToBattle(Character character)
    {
        var spawnPoint = GetSpawnPoint(character.characterType);
        
        // If a SpawnPoint is available, add the character to the battle:
        if (spawnPoint != null)
        {
            character.SetSpawn(spawnPoint);
            characters.Add(character);

            characters = characters.OrderBy(c => c.Speed).Reverse().ToList();
        }
        // Else remove the character from existance:
        else
        {
            Destroy(character.gameObject);
        }
    }

    /// <summary>
    /// Removes a Character from battle.
    /// </summary>
    /// <param name="character">The Character to remove.</param>
    public void RemoveCharacterFromBattle(Character character)
    {
        characters.Remove(character);
    }

    /// <summary>
    /// Adds a BattleAction at the end of the queue.
    /// </summary>
    /// <param name="action">The action to enqueue.</param>
    public void EnqueueAction(BattleAction action)
    {
        battleActions.Enqueue(action);
    }
    
    /// <summary>
    /// Invokes the enqueued actions.
    /// </summary>
    private void Update()
    {
        if (!battleStarted)
        {
            // DEBUG!
            if (Input.GetKeyDown(KeyCode.Space) ||
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                LoadBattle();
                GlobalMessage.instance.BroadCastMessage("Battle started!", 3);
            }
            //

            return;
        }

        // Find next action to perform.
        if (battleActions.Count > 0 && (debugSpeed || Time.time >= timeTillNextAction))
        {
            var action = battleActions.Dequeue();
            action.method();

            timeTillNextAction = Time.time + action.duration;
        }
    }
    
    /// <summary>
    /// Finds the next character up from the currentTurnCharacter in the list.
    /// </summary>
    private void StartNextTurn()
    {
        var index = (characters.FindIndex(c => c == currentTurnCharacter) + 1) % characters.Count;
        currentTurnCharacter = characters[index];
        currentTurnCharacter.InitializeTurn();
    }

    /// <summary>
    /// Ends the turn and passes it onto the next character.
    /// </summary>
    public void EndTurn()
    {
        DebugHelper.instance.AddMessage(string.Format("{0} ended their turn.", currentTurnCharacter.name));
        EnqueueAction(new BattleAction(StartNextTurn, 0));
    }

    /// <summary>
    /// Checks if either all allies have died, or all the enemies have died.
    /// If so, the battle has ended.
    /// </summary>
    public void TryEndBattle()
    {
        var allFriendlyDead = GetCharactersOfType(Character.CharacterType.Friendly)
            .Aggregate(true, (current, friendly) => current & friendly.IsDead);
        var allEnemyDead = GetCharactersOfType(Character.CharacterType.Enemy).Count == 0;

        if (allEnemyDead || allFriendlyDead)
        {
            var whoDied = allEnemyDead ? "Foes" : "Allies";
            DebugHelper.instance.AddMessage(string.Format("{0} have been slain. The battle has ended.", whoDied), 5);

            battleActions.Clear();
            currentTurnCharacter = null;
            foreach (var spawnPoint in spawnPoints)
            {
                spawnPoint.taken = false;
            }

            // TODO: Remove summoned allies

            battleStarted = false;

            GameObject.FindWithTag("Player").GetComponent<Player>().Save();
        }
    }

    /// <summary>
    /// Returns a list of all the characters currently in the battle of the given type.
    /// </summary>
    /// <param name="type">The type to match the characters to.</param>
    public List<Character> GetCharactersOfType(Character.CharacterType type)
    {
        return type == Character.CharacterType.All ? characters : characters.FindAll(c => c.characterType == type);
    }
}

/// <summary>
/// A nullable type for SpawnPoint information. This includes its position, availability and type.
/// </summary>
[Serializable]
public class SpawnPoint
{
    public Transform transform;
    public Character.CharacterType availableToCharactersOfType;
    public bool taken;
}
