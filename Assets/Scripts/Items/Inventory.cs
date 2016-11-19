using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    public List<Item> items;

    public Inventory()
    {
        items = new List<Item>();
    }

    /// <summary>
    /// Adds an Item to this inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
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
            item.SetInventory(this);
            items.Add(item);
        }  
    }

    /// <summary>
    /// Removes an Item to this inventory.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
}