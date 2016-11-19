using System;
using UnityEngine;

// TODO: These items are hardcoded to only work with the player character.

[Serializable]
public abstract class Potion : Item
{
    protected readonly int healsFor;

    /// <summary>
    /// An item that heals the user for a set amount.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <param name="amount">The amount of uses the item has.</param>
    /// <param name="healsFor">For how much the potion heals its user.</param>
    protected Potion(string name, int amount, int healsFor)
        : base(name, amount)
    {
        this.healsFor = healsFor;
    }
}

[Serializable]
public class HealthPotion : Potion
{
    /// <summary>
    /// A potion that heals its user's health for 100.
    /// </summary>
    /// <param name="amount">The amount of uses this potion has.</param>
    public HealthPotion(int amount)
        : base("Health potion", amount, 100)
    { }

    /// <summary>
    /// Heals the player's health for 100.
    /// </summary>
    protected override void ItemFunction(Character user)
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            DebugHelper.instance.AddMessage(string.Format("{0} restored {1} health", name, healsFor));
            
            user.AddHealth(healsFor);
        }, 2));
    }
}

[Serializable]
public class ManaPotion : Potion
{    
    /// <summary>
     /// A potion that heals its user's mana for 100.
     /// </summary>
     /// <param name="amount">The amount of uses this potion has.</param>
    public ManaPotion(int amount) 
        : base("Mana potion", amount, 100)
    { }

    /// <summary>
    /// Heals the player's mana for 100.
    /// </summary>
    protected override void ItemFunction(Character user)
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            DebugHelper.instance.AddMessage(string.Format("{0} restored {1} mana", name, healsFor));

            user.AddMana(healsFor);
        }, 2));
    }
}