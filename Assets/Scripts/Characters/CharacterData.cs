using System;

[Serializable]
public class CharacterData
{
    public float MaxHealth { get; private set; }
    public float Strength { get; private set; }
    public float Intelligence { get; private set; }
    public float Speed { get; private set; }

    public CharacterData(float maxHealth, float str, float inte, float speed)
    {
        MaxHealth = maxHealth;
        Strength = str;
        Intelligence = inte;
        Speed = speed;
    }
}