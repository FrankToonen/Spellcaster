using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// A static class that contain references to each XmlContainer object.
/// </summary>
public static class XmlContainers
{
    public static AttackContainer attackContainer = LoadXml<AttackContainer>(Resources.Load<TextAsset>("Xml/Attacks").text);

    /// <summary>
    /// Loads a XmlContainer of type T.
    /// </summary>
    /// <typeparam name="T">The explicit type of the container.</typeparam>
    /// <param name="text">The Xml data to parse.</param>
    /// <returns>The XmlContainer object.</returns>
    public static T LoadXml<T>(string text)
    {
        var serializer = new XmlSerializer(typeof(AttackContainer));
        var container = (T)serializer.Deserialize(new StringReader(text));

        return container;
    }
}

/// <summary>
/// A container class that loads in all available attacks for characters to use.
/// </summary>
[XmlRoot("AttackContainer")]
public class AttackContainer
{
    [XmlArray("Attacks"), XmlArrayItem("AttackInfo")]
    public AttackInfo[] attacks;

    /// <summary>
    /// Retrieves an AttackInfo with the given name.
    /// </summary>
    /// <param name="name">The name for the AttackInfo to get.</param>
    /// <returns>The desired AttackInfo.</returns>
    public AttackInfo GetAttack(string name)
    {
        foreach (var attack in attacks)
        {
            if (attack.name == name)
            {
                return attack;
            }
        }

        throw new Exception("Attack does not exist: " + name);
    }
}