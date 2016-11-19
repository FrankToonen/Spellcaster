using UnityEngine;

/// <summary>
/// Applies the inflictions of the AttackInfo.
/// Checks for mana cost to be able to be used.
/// </summary>
public class BaseAttack
{
    public AttackInfo info;

    public BaseAttack(AttackInfo info)
    {
        this.info = info;
    }

    /// <summary>
    /// Performs the desired action on the given target.
    /// </summary>
    /// <param name="target">The target to use this attack on.</param>
    public virtual void UseAttack(Character target)
    {
        if (info.inflictionInfo != null)
        {
            target.ApplyInfliction(info.inflictionInfo.inflictionType, info.inflictionInfo.inflictionDuration, info.inflictionInfo.inflictionDamage);
        }
    }

    /// <summary>
    /// Whether or not the attack can be used.
    /// </summary>
    /// <param name="user">The character trying to use this attack.</param>
    /// <returns>Whether this attack can be used.</returns>
    public virtual bool CanBeUsed(Character user)
    {
        return user.Mana >= info.manaCost;
    }
}

/// <summary>
/// Deals damage to the target.
/// </summary>
public class DamageAttack : BaseAttack
{
    public DamageAttack(AttackInfo info) 
        : base(info)
    { }

    public override void UseAttack(Character target)
    {
        base.UseAttack(target);
        target.AddHealth(-info.damage);
    }
}

/// <summary>
/// Heals the target.
/// </summary>
public class HealAttack : BaseAttack
{
    public HealAttack(AttackInfo info)
        : base(info)
    { }

    public override void UseAttack(Character target)
    {
        base.UseAttack(target);
        target.AddHealth(info.damage);
    }
}

/// <summary>
/// Summons a Character (given by the specific subclass).
/// Checks for available SpawnPoints (in addition to the mana cost) to be able to be used.
/// </summary>
public abstract class SummonAttack : BaseAttack
{
    protected GameObject summonPrefab;

    protected SummonAttack(AttackInfo info, GameObject summonPrefab)
        : base(info)
    {
        this.summonPrefab = summonPrefab;
    }

    public override void UseAttack(Character target)
    {
        base.UseAttack(target);

        // Instantiate a new character and make it of the same type as the given target.
        var instance = GameObject.Instantiate(summonPrefab, BattleManager.instance.transform);
        var character = instance.GetComponent<Character>();
        character.characterType = target.characterType;

        // Add it to the battle.
        BattleManager.instance.AddCharacterToBattle(character);
    }

    public override bool CanBeUsed(Character user)
    {
        return base.CanBeUsed(user) && BattleManager.instance.CanSpawn(user.characterType);
    }
}

/// <summary>
/// TODO: Is a subclass really necessarry if the attack factory makes a special case for this attack anyways?
/// Summons a Skeleton.
/// </summary>
public class SummonSkeleton : SummonAttack
{
    public SummonSkeleton(AttackInfo info) 
        : base(info, Resources.Load<GameObject>("Prefabs/Summons/pf_skeleton"))
    { }
}

