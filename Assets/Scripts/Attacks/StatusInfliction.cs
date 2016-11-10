using System;
using UnityEngine;

public static class StatusInflictionFactory
{
    /// <summary>
    ///  The type of infliction. This is used to create the correct subclass.
    /// </summary>
    public enum InflictionType
    {
        Freeze,
        Poison,
        ManaHeal,
        HealthHeal,
        None
    }

    /// <summary>
    /// Creates an instance of a StatusInfliction, based on the given InflictionType.
    /// </summary>
    /// <param name="type">Which StatusInfliction to create.</param>
    /// <param name="duration">The amount of turns the StatusInfliction should last.</param>
    /// <param name="damage">The amount of damage the StatusInfliction should deal.</param>
    /// <returns>An instance of a StatusInfliction.</returns>
    public static StatusInfliction GetStatusInfliction(InflictionType type, int duration, int damage)
    {
        switch (type)
        {
            case InflictionType.Freeze:
            {
                return new FreezeInfliction(duration);
            }
            case InflictionType.Poison:
            {
                return new PoisonInfliction(duration, damage);
            }
            case InflictionType.HealthHeal:
            {
                    return new HealthHealInfliction(duration, damage);
                }
                case InflictionType.ManaHeal:
            {
                    return new ManaHealInfliction(duration, damage);
            }
            case InflictionType.None:
            {
                // This means the attack didn't cause a StatusInfliction.
                return null;
            }
            default:
            {
                throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}

/// <summary>
/// The base functionality for StatusInflictions. 
/// This handles the updating of the duration, merging and visualization of the indicator.
/// This will be applied at the start of the turn.
/// </summary>
public abstract class StatusInfliction
{
    // An event invoked whenever the duration becomes <= 0.
    public event Action OnExpire;

    // How often a StatusInfliction can stack.
    private const int MAXSTACKS = 5;

    // How many stacks this StatusInfliction has. This will increase the damage of this StatusInfliction.
    private int stacks;

    // How many turns this StatusInfliction remains.
    private int duration;

    // The damage one stack provides.
    private int damagePerStack;

    // The message this StatusInfliction shows when applied.
    private readonly string messageText;

    // The indicator GameObject associated with this StatusInfliction.
    public GameObject indicator;

    // The indicator text for the amount of stacks of this StatusInfliction.
    private readonly TextMesh stacksText;

    // The indicator text for the duration left on this StatusInfliction.
    private readonly TextMesh durationText;

    /// <summary>
    /// Creates an instance of an indicator GameObject and initializes all the variables.
    /// </summary>
    /// <param name="duration">How many rounds this StatusInfliction lasts.</param>
    /// <param name="damage">How much damage a stack of this StatusInfliction deals.</param>
    /// <param name="messageText">The text to show when this StatusInfliction is applied.</param>
    protected StatusInfliction(int duration, int damage, string messageText)
    {
        var indicatorPrefab = Resources.Load<GameObject>("Prefabs/StatusInflictions/" + this);
        if (indicatorPrefab == null)
        {
            throw new Exception(string.Format("Prefab for {0} does not exist!", this));
        }

        indicator = GameObject.Instantiate(indicatorPrefab);

        stacksText = indicator.transform.FindChild("Multiplier").GetComponent<TextMesh>();
        durationText = indicator.transform.FindChild("Duration").GetComponent<TextMesh>();

        Duration = duration;
        Stacks = 1;
        damagePerStack = damage;
        this.messageText = messageText;
    }

    /// <summary>
    /// Subtracts the duration and broadcasts the message.
    /// </summary>
    /// <param name="character">The character to apply this infliction to.</param>
    public virtual void ApplyInflictionTo(Character character)
    {
        Duration--;

        if (Duration <= 0)
        {
            if (OnExpire != null)
            {
                OnExpire.Invoke();
            }
        }

        GlobalMessage.instance.BroadCastMessage(MessageText, 2);

        DebugHelper.instance.AddMessage(string.Format("{0} was inflicted with: {1}", character.name, MessageText));
    }

    /// <summary>
    /// Merge two StatusInfliction by taking the biggest duration and adding a stack. 
    /// This also takes the biggest damagePerStack.
    /// </summary>
    /// <param name="infliction">The StatusInfliction to be merged with this.</param>
    public void AddDuplicateInfliction(StatusInfliction infliction)
    {
        Duration = Mathf.Max(infliction.Duration, Duration);
        Stacks = Mathf.Clamp(Stacks + 1, 0, MAXSTACKS);
        damagePerStack = Mathf.Max(infliction.damagePerStack, damagePerStack);

        DebugHelper.instance.AddMessage(string.Format("Merge inflictions of type {0}: {1}", this, Stacks));
    }
    
    /// <summary>
    /// The duration of this StatusInfliction. Setting this will automatically update the durationText.
    /// </summary>
    protected int Duration
    {
        get { return duration; }
        private set
        {
            duration = value;
            durationText.text = duration.ToString();
        }
    }

    /// <summary>
    /// The amount of stacks of this StatusInfliction. Setting this will automatically update the stacksText.
    /// </summary>
    protected int Stacks
    {
        get { return stacks; }
        private set
        {
            stacks = value;
            stacksText.text = StacksText;
        }
    }

    /// <summary>
    /// The total damage this StatusInfliction deals.
    /// </summary>
    protected int Damage
    {
        get { return damagePerStack*Stacks; }
    }

    /// <summary>
    /// The messageText interpolated with the damage.
    /// </summary>
    protected string MessageText
    {
        get { return string.Format(messageText, Damage); }
    }

    /// <summary>
    /// The text for the inflictionText TextMesh. This can be overridden to show something else.
    /// </summary>
    protected virtual string StacksText
    {
        get { return "x" + Stacks; }
    }
}

/// <summary>
/// This StatusInfliction freezes the character, making it unable to do anything this turn.
/// </summary>
public class FreezeInfliction : StatusInfliction
{
    /// <summary>
    /// Sets its duration, damage, type and messageText.
    /// </summary>
    public FreezeInfliction(int duration)
        : base(duration, 0, "Frozen")
    { }

    /// <summary>
    /// Sets the character to be unable to do anything this turn.
    /// </summary>
    public override void ApplyInflictionTo(Character character)
    {
        // Apply this immediately, to prevent it from attacking anyway due to Queue timing.
        character.canAttack = false;

        // Enqueue a new action that simply calls the base of this method.
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            base.ApplyInflictionTo(character);
        }, 2));
    }

    /// <summary>
    /// Do not show anything, as stacking this infliction doesn't do anything.
    /// </summary>
    protected override string StacksText
    {
        get { return ""; }
    }
}

/// <summary>
/// This StatusInfliction poisons the character, dealing damage to it.
/// </summary>
public class PoisonInfliction : StatusInfliction
{    
     /// <summary>
     /// Sets its duration, damage, type and messageText.
     /// </summary>
    public PoisonInfliction(int duration, int damage) 
        : base(duration, damage, "Poisoned ({0})")
    { }

    /// <summary>
    /// Enqueue a new action that calls the base of this method and deals damage to the character.
    /// </summary>
    public override void ApplyInflictionTo(Character character)
    {

        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            base.ApplyInflictionTo(character);
            character.AddHealth(-Damage);
        }, 2));
    }
}

/// <summary>
/// This StatusInfliction heals the health of the character.
/// </summary>
public class HealthHealInfliction : StatusInfliction
{
    /// <summary>
    /// Sets its duration, damage, type and messageText.
    /// </summary>
    public HealthHealInfliction(int duration, int damage) 
        : base(duration, damage, "Blessing of health {0}")
    { }

    /// <summary>
    /// Enqueue a new action that calls the base of this method and heals the health of the character.
    /// </summary>
    public override void ApplyInflictionTo(Character character)
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            base.ApplyInflictionTo(character);
            character.AddHealth(Damage);
        }, 2));
    }
}

/// <summary>
/// This StatusInfliction heals the mana of the character.
/// </summary>
public class ManaHealInfliction : StatusInfliction
{
    /// <summary>
    /// Sets its duration, damage, type and messageText.
    /// </summary>
    public ManaHealInfliction(int duration, int damage)
        : base(duration, damage, "Blessing of mana {0}")
    { }

    /// <summary>
    /// Enqueue a new action that calls the base of this method and heals the mana of the character.
    /// </summary>
    public override void ApplyInflictionTo(Character character)
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            base.ApplyInflictionTo(character);
            character.AddMana(Damage);
        }, 2));
    }
}