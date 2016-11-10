using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private CameraAnimationInfo test;

    public const string FILENAME = "Player";

    [SerializeField] private PlayerTurnMenu attackUI;
    
    private List<Item> items = new List<Item>();

    protected override void StartTurn()
    {
        if (!canAttack)
        {
            return;
        }

        BattleManager.instance.EnqueueAction(new BattleAction(() => 
        {
            attackUI.CreateAttackButtons(attacks, this);
            attackUI.CreateItemButtons(items);
            attackUI.gameObject.SetActive(true);
        }, 0));

    }

    // DEBUG
    private void Start()
    {
        AddItem(new HealthPotion(5));
        AddItem(new ManaPotion(3));
    }       
    //

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CameraController.instance.PlayCameraAnimation(test);
        }
    }

    /// <summary>
    /// Loads the character stats.
    /// TODO: Make a better save file for the player, including its inventory, health, mana and attacks.
    /// </summary>
    protected override void Reset()
    {
        stats = GameManager.LoadFile<CharacterData>(FILENAME + "Stats");

        var saveData = GameManager.LoadFile<PlayerSaveData>(FILENAME + "Save");
        Health = saveData.currentHealth;
        Mana = saveData.currentMana;
        items = saveData.items;
    }

    public void AddItem(Item item)
    {
        // Try increase the count if it already exists.
        var existingItem = items.Find(i => i.name == item.name);
        if (existingItem != null)
        {
            existingItem.amount += item.amount;
        }
        // Else add it as a new item.
        else
        {
            items.Add(item);
        }
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public void Save()
    {
        GameManager.SaveFile(FILENAME + "Stats", stats);
        GameManager.SaveFile(FILENAME + "Save", new PlayerSaveData(Health, Mana, items));
    }
}

[Serializable]
public struct PlayerSaveData
{
    public int currentHealth;
    public int currentMana;
    public List<Item> items;

    public PlayerSaveData(int currentHealth, int currentMana, List<Item> items)
    {
        this.currentHealth = currentHealth;
        this.currentMana = currentMana;
        this.items = items;
    }
}
