using System;

/// <summary>
/// A factory that generates attacks to be used by Characters.
/// </summary>
public class AttackFactory
{
    public static BaseAttack GetAttack(string name)
    {
        switch (name)
        {
            case "Mana well":
            case "Healing mist":
                {
                return new BaseAttack(XmlContainers.attackContainer.GetAttack(name));
                }
            case "Poison fang":
            case "Moonfire":
            case "Fireball":
            case "Flamestrike":
            case "Hellfire":
            case "Frostbolt":
            {
                return new DamageAttack(XmlContainers.attackContainer.GetAttack(name));
            }
            case "Healing touch":
            {
                return new HealAttack(XmlContainers.attackContainer.GetAttack(name));
            }
            case "Summon skeleton":
            {
                return new SummonSkeleton(XmlContainers.attackContainer.GetAttack(name));
            }
            default:
            {
                throw new Exception("Attack does not exist: " + name);
            }
        }
    }
}