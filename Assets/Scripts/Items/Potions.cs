using System;
using UnityEngine;

[Serializable]
public abstract class Potion : Item
{
    protected readonly int healsFor;

    protected Potion(string name, int amount, int healsFor)
        : base(name, amount)
    {
        this.healsFor = healsFor;
    }
}

[Serializable]
public class HealthPotion : Potion
{
    public HealthPotion(int amount)
        : base("Health potion", amount, 100)
    { }

    protected override void ItemFunction()
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            DebugHelper.instance.AddMessage(string.Format("{0} restored {1} health", name, healsFor));
            GameObject.FindWithTag("Player").GetComponent<Character>().AddHealth(healsFor);
        }, 2));
    }
}

[Serializable]
public class ManaPotion : Potion
{
    public ManaPotion(int amount) 
        : base("Mana potion", amount, 100)
    { }

    protected override void ItemFunction()
    {
        BattleManager.instance.EnqueueAction(new BattleAction(() =>
        {
            DebugHelper.instance.AddMessage(string.Format("{0} restored {1} mana", name, healsFor));
            GameObject.FindWithTag("Player").GetComponent<Character>().AddMana(healsFor);
        }, 2));
    }
}