using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public enum CharacterType
    {
        Friendly,
        Enemy,
        All,
        None
    }

    public enum HighlightType
    {
        Option,
        Selected,
        None
    }

    public event Action OnStartTurn;

    public delegate void HighlightClicked(Character target);
    public event HighlightClicked OnHighlightClicked;

    public CharacterType characterType;
    [SerializeField] protected CharacterData stats;
    protected SpawnPoint spawnPoint;

    [SerializeField] private string[] availableAttacks;
    protected BaseAttack[] attacks;

    private HighlightType highlightType = HighlightType.None;
    [SerializeField] private Animator highlight;
    private bool isHighlighted;

    [SerializeField] private Vector3 healthbarOffset;
    private Healthbar healthbar;
    private int health;

    [SerializeField] private Vector3 manabarOffset;
    private Healthbar manabar;
    protected int mana;

    [SerializeField] private Vector2 inflictionStartPosition;
    private List<StatusInfliction> statusInflictions;
    public bool canAttack;

    /// <summary>
    /// Instantiates the healthbar and manabar objects and calls the reset function.
    /// </summary>
    protected virtual void Awake()
    {
        attacks = new BaseAttack[availableAttacks.Length];
        for (var i = 0; i < availableAttacks.Length; i++)
        {
            attacks[i] = AttackFactory.GetAttack(availableAttacks[i]);
        }
        
        var healthbarObject = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Healthbar_bar"), transform) as GameObject;
        healthbar = healthbarObject.GetComponent<Healthbar>();
        healthbar.Initialize(transform.position + healthbarOffset, Color.red);

        var manabarObject = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Healthbar_bar"), transform) as GameObject;
        manabar = manabarObject.GetComponent<Healthbar>();
        manabar.Initialize(transform.position + manabarOffset, Color.blue);

        statusInflictions = new List<StatusInfliction>();
        OnStartTurn += HandleInflictions;
        OnStartTurn += delegate { BattleManager.instance.EnqueueAction(new BattleAction(StartTurn, 0)); };

        Reset();
    }

    /// <summary>
    /// Sets this character's health and mana to full.
    /// </summary>
    protected virtual void Reset()
    {
        Health = stats.maxHealth;
        Mana = stats.maxMana;
    }

    /// <summary>
    /// Sets the spawnpoint for this Character.
    /// </summary>
    /// <param name="spawnPoint">The spawnpoint to set this Character to.</param>
    public void SetSpawn(SpawnPoint spawnPoint)
    {
        transform.position = spawnPoint.transform.position;
        spawnPoint.taken = true;

        this.spawnPoint = spawnPoint;
    }

    /// <summary>
    /// Is called whenever this character's turn starts.
    /// </summary>
    public void InitializeTurn()
    {
        if (OnStartTurn != null)
        {
            DebugHelper.instance.AddMessage(string.Format("{0} started their turn.", name));
            canAttack = true;
            OnStartTurn.Invoke();
        }
    }

    protected abstract void StartTurn();

    /// <summary>
    /// Iterates over each StatusInfliction inflicted on this Character and applies them.
    /// </summary>
    private void HandleInflictions()
    {
        foreach (var infliction in statusInflictions)
        {
            infliction.ApplyInflictionTo(this);
        }
    }

    /// <summary>
    /// Sets the HighlightType type on this character. This will change the color / visibility of the highlight object attached.
    /// </summary>
    /// <param name="type">The HighlightType to set this to.</param>
    public void SetHighlight(HighlightType type)
    {
        highlightType = type;
        isHighlighted = highlightType != HighlightType.None;
        
        highlight.Play(type.ToString(), gameObject.layer, 0);
    }

    /// <summary>
    /// Whenever the character is highlighted and is available, clicking it will select it as the attack's target.
    /// </summary>
    private void OnMouseDown()
    {
        if (isHighlighted && highlightType == HighlightType.Option && OnHighlightClicked != null)
        {
            OnHighlightClicked.Invoke(this);
        }
    }

    /// <summary>
    /// Enqueues an attack in the BattleManager.
    /// </summary>
    public void PerformAttack(BaseAttack attack, List<Character> targets)
    {
        Mana -= attack.info.manaCost;
        
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            // DEBUG
            var debugTargetString = targets.Aggregate("", (current, target) => current + target.name + ", ");
            debugTargetString = debugTargetString.Substring(0, debugTargetString.Length - 2);
            DebugHelper.instance.AddMessage(string.Format("{0} used {1} dealing {2} damage to: {3}", name, attack.info.name, attack.info.damage, debugTargetString));
            // DEBUG

            foreach (var target in targets)
            {
                attack.UseAttack(target);
            }

            PlayAttackAnimation(targets);

            BattleManager.instance.EndTurn();
        }, 2));
    }

    /// <summary>
    /// Plays an animation moving the camera from caster to target.
    /// TODO: Make more betterest (generic and what not).
    /// </summary>
    /// <param name="targets">The targets for the attack.</param>
    private void PlayAttackAnimation(IList<Character> targets)
    {
        var targetPosition = targets[0].transform.position;
        targetPosition.y = 3;
        var controlPoints = new List<Vector2> {transform.position, transform.position, targetPosition, targetPosition};

        var scaleCurve = new AnimationCurve(new Keyframe(0, 0.5f));

        var cameraAnimation = new CameraAnimationInfo(controlPoints, 1, scaleCurve);

        CameraController.instance.PlayCameraAnimation(cameraAnimation);
    }

    /// <summary>
    /// Adds a StatusInfliction to this character. If one such infliction already exists, combine them.
    /// Combining the infliction adds up their damage and takes the longest duration.
    /// </summary>
    /// <param name="type">The type of infliction</param>
    /// <param name="duration">The duration for the infliction.</param>
    /// <param name="damage">The damage the infliction deals.</param>
    public void ApplyInfliction(StatusInflictionFactory.InflictionType type, int duration, int damage)
    {
        var newInfliction = StatusInflictionFactory.GetStatusInfliction(type, duration, damage);
        if (newInfliction == null)
        {
            // The attack did not cause a status infliction.
            return;
        }

        var existingInfliction = statusInflictions.Find(i => i.GetType() == newInfliction.GetType());
        if (existingInfliction != null)
        {
            Destroy(newInfliction.indicator);
            existingInfliction.AddDuplicateInfliction(newInfliction);
        }
        else
        {
            newInfliction.indicator.transform.SetParent(transform);
            newInfliction.indicator.transform.localPosition = inflictionStartPosition + new Vector2(statusInflictions.Count* (.4f / transform.localScale.x), 0);
            newInfliction.OnExpire += () => { RemoveInfliction(newInfliction); };

            statusInflictions.Add(newInfliction);
        }
    }

    /// <summary>
    /// Removes the given infliction from the character.
    /// </summary>
    /// <param name="infliction">The infliction to remove.</param>
    public void RemoveInfliction(StatusInfliction infliction)
    {
        // Start a coroutine to apply this effect after 2 seconds have passed.
        StartCoroutine(RemoveIndicator(infliction));
    }

    /// <summary>
    /// Removes the given StatusInfliction after 2 seconds (when the effect has been applied).
    /// </summary>
    /// <param name="infliction">The StatusInfliction to remove.</param>
    private IEnumerator RemoveIndicator(StatusInfliction infliction)
    {
        // TODO: Remove hardcoded 2. This should be the time the infliction message is broadcasted (Make that a const ?).
        yield return new WaitForSeconds(2);

        Destroy(infliction.indicator);
        statusInflictions.Remove(infliction);

        for (var i = 0; i < statusInflictions.Count; i++)
        {
            statusInflictions[i].indicator.transform.localPosition = inflictionStartPosition + new Vector2(i * (.4f / transform.localScale.x), 0);
        }
    }
    
    /// <summary>
    /// Add the given amount to this character's health.
    /// </summary>
    public void AddHealth(int amount)
    {
        Health += amount;
        healthbar.ShowDamage(-amount);
    }

    /// <summary>
    /// Add the given amount to this character's mana.
    /// </summary>
    public void AddMana(int amount)
    {
        Mana += amount;
        manabar.ShowDamage(-amount);
    }

    /// <summary>
    /// This method is called whenever this character dies.
    /// </summary>
    protected virtual void Die()
    {
        spawnPoint.taken = false;

        BattleManager.instance.RemoveCharacterFromBattle(this);
        BattleManager.instance.TryEndBattle();
    }

    /// <summary>
    /// Converts the TargetType to a CharacterType, based on the given CharacterType.
    /// </summary>
    /// <param name="characterType">The CharacterType to base the results off of.</param>
    /// <param name="targetType">The TargetType to convert.</param>
    /// <returns>The desired CharacterType.</returns>
    public static CharacterType TargetToCharacterType(CharacterType characterType, AttackInfo.TargetType targetType)
    {
        switch (targetType)
        {
            case AttackInfo.TargetType.SingleFriendly:
            case AttackInfo.TargetType.AllFriendly:
            {
                return characterType;
            }
            case AttackInfo.TargetType.SingleEnemy:
            case AttackInfo.TargetType.AllEnemy:
            {
                return characterType == CharacterType.Friendly ? CharacterType.Enemy : CharacterType.Friendly;
            }
            case AttackInfo.TargetType.Everyone:
            {
                return CharacterType.All;
            }
            case AttackInfo.TargetType.None:
            {
                return CharacterType.None;
            }
            default:
            {
                throw new ArgumentOutOfRangeException("targetType", targetType, null);
            }
        }
    }

    #region Properties
    /// <summary>
    /// The health of this character. Setting it will update the healthbar.
    /// </summary>
    public int Health
    {
        get { return health; }
        protected set
        {
            health = Mathf.Clamp(value, 0, stats.maxHealth);
            healthbar.SetHealthBar(health, stats.maxHealth);

            if (health <= 0)
            {
                canAttack = false;
                Die();
            }
        }
    }

    /// <summary>
    /// The mana of this character. Setting it will update the manabar.
    /// </summary>
    public int Mana
    {
        get { return mana; }
        protected set
        {
            mana = Mathf.Clamp(value, 0, stats.maxMana);
            manabar.SetHealthBar(mana, stats.maxMana);
        }
    }

    /// <summary>
    /// A getter for the stats.speed variable.
    /// </summary>
    public int Speed
    {
        get { return stats.speed; }
    }

    /// <summary>
    /// Returns whether the character has died.
    /// </summary>
    public bool IsDead
    {
        get { return health <= 0; }
    }
    #endregion
}
