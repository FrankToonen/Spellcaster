using System;
using UnityEngine;

public class AttackInfo : IComparable
{
    public enum TargetType
    {
        SingleFriendly,
        AllFriendly,
        SingleEnemy,
        AllEnemy,
        Everyone,
        None
    }

    public string name = "This name was not loaded.";
    public string text = "this text was not loaded.";
    public int manaCost = 9999;

    // TODO: Split damage in physical & elemental damage?
    public int damage;
    public TargetType targetType;

    // TODO: Make this an array to allow multiple inflictions per attack?
    public StatusInflictionInfo inflictionInfo;

    /// <summary>
    /// Returns the mana cost + the attack text. {0} is the damage (embedded in the text variable), {1} is the manacost.
    /// </summary>
    public string AttackText()
    {
        return string.Format("{1} mana: " + text, Mathf.Abs(damage), manaCost);
    }

    /// <summary>
    /// Returns the name of the attack.
    /// </summary>
    public override string ToString()
    {
        return name;
    }

    /// <summary>
    /// Compares the instance with the given object. This object is expected to be of type string.
    /// </summary>
    public int CompareTo(object other)
    {
        return name.CompareTo(other);
    }

    /// <summary>
    /// A nullable container for StatusInfliction data.
    /// </summary>
    public class StatusInflictionInfo
    {
        public StatusInflictionFactory.InflictionType inflictionType;
        public int inflictionDamage;
        public int inflictionDuration;
    }
}
