using System;

[Serializable]
public struct CharacterData
{
    public int maxHealth;
    public int maxMana;
    public int strength;
    public int intelligence;
    public int speed;

    public CharacterData(int maxHealth, int maxMana, int str, int inte, int speed)
    {
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;
        strength = str;
        intelligence = inte;
        this.speed = speed;
    }
}
