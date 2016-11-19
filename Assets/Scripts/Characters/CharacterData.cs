using System;

[Serializable]
public struct CharacterData
{
    public int maxHealth;
    public int maxMana;
    public int strength;
    public int intelligence;
    public int speed;

    /// <summary>
    /// The stats of a character.
    /// </summary>
    /// <param name="maxHealth">Their maximum health.</param>
    /// <param name="maxMana">Their maximum mana.</param>
    /// <param name="str">???</param>
    /// <param name="inte">???</param>
    /// <param name="speed">Their speed which determines turn order.</param>
    public CharacterData(int maxHealth, int maxMana, int str, int inte, int speed)
    {
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;
        strength = str;
        intelligence = inte;
        this.speed = speed;
    }
}
