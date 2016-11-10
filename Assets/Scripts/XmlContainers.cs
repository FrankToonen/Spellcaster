using UnityEngine;

public static class XmlContainers
{
    public static AttackContainer attackContainer = AttackContainer.LoadXml(Resources.Load<TextAsset>("Xml/Attacks").text);
}