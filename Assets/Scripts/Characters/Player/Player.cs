using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public const string FILENAME = "Player";
    
    [SerializeField] private PlayerTurnMenu attackUI;
    private Inventory inventory = new Inventory();

    /// <summary>
    /// Opens the PlayerTurnMenu which allows the player to choose an attack.
    /// </summary>
    protected override void StartTurn()
    {
        if (!canAttack)
        {
            return;
        }

        BattleManager.instance.EnqueueAction(new BattleAction(() => 
        {
            attackUI.CreateAttackButtons(attacks, this);
            attackUI.CreateItemButtons(inventory.items, this);
            attackUI.gameObject.SetActive(true);
        }, 0));
    }
    
    /// <summary>
    /// Loads the character stats.
    /// TODO: Make a better save file for the player, including its inventory, health, mana and attacks.
    /// </summary>
    protected override void Reset()
    {
        var saveData = GameManager.LoadFile<PlayerSaveData>(FILENAME + "Save");
        stats = saveData.stats;
        Health = saveData.currentHealth;
        Mana = saveData.currentMana;
        foreach (var item in saveData.currentInventoryItems)
        {
            inventory.AddItem(item);
        }
    }

    /// <summary>
    /// Adds an item to the player's inventory.
    /// </summary>
    /// <param name="item">The Item to add to the inventory.</param>
    public void AddItem(Item item)
    {
        inventory.AddItem(item);
    }

    /// <summary>
    /// Removes an item from the player's inventory.
    /// </summary>
    /// <param name="item">The Item to remove from the inventory.</param>
    public void RemoveItem(Item item)
    {
        inventory.RemoveItem(item);
    }

    /// <summary>
    /// Saves the state of the player character.
    /// </summary>
    public void Save()
    {
        GameManager.SaveFile(FILENAME + "Save", new PlayerSaveData(stats, Health, Mana, inventory.items));
    }
}

/// <summary>
/// The save data for the player.
/// This includes their health, mana and inventory state.
/// </summary>
[Serializable]
public struct PlayerSaveData
{
    public CharacterData stats;
    public int currentHealth;
    public int currentMana;
    public List<Item> currentInventoryItems;

    public PlayerSaveData(CharacterData stats, int currentHealth, int currentMana, List<Item> currentInventoryItems)
    {
        this.stats = stats;
        this.currentHealth = currentHealth;
        this.currentMana = currentMana;
        this.currentInventoryItems = currentInventoryItems;
    }
}
