using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

// TODO: Split damage in physical & elemental damage

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

[XmlRoot("AttackContainer")]
public class AttackContainer
{
    [XmlArray("Attacks"), XmlArrayItem("AttackInfo")] public AttackInfo[] attacks;

    public static AttackContainer LoadXml(string text)
    {
        var serializer = new XmlSerializer(typeof(AttackContainer));
        var container = serializer.Deserialize(new StringReader(text)) as AttackContainer;
        Array.Sort(container.attacks, (a1, a2) => string.CompareOrdinal(a1.name, a2.name));

        return container;
    }

    /// <summary>
    /// Retrieves an AttackInfo with the given name.
    /// </summary>
    /// <param name="name">The name for the AttackInfo to get.</param>
    /// <returns>The desired AttackInfo.</returns>
    public AttackInfo GetAttack(string name)
    {
        var index = Array.BinarySearch(attacks, name);
        if (index >= 0 && index < attacks.Length)
        {
            return attacks[index];
        }

        // TODO: Foreach loop is faster than binary search (at 10 attacks).
        //foreach (var attack in attacks)
        //{
        //    if (attack.name == name)
        //    {
        //        return attack;
        //    }
        //}

        throw new Exception("Attack does not exist: " + name);
    }
}
