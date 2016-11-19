using System;
using UnityEngine;

[Serializable]
public abstract class Item
{
    // TODO: This delegate restricts items to only be usable on the user and not have them be targetable.
    public delegate void OnItemUse(Character user);
    private event OnItemUse OnUseItem;

    // The inventory this item exists in.
    protected Inventory inventory;

    public string name;
    public int amount;

    /// <summary>
    /// An item that has a use function and an amount of uses before depletion.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <param name="amount">How often the item can be used.</param>
    protected Item(string name, int amount)
    {
        this.name = name;
        this.amount = amount;

        OnUseItem += delegate { DecreaseAmount(); };
        OnUseItem += ItemFunction;
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    /// <summary>
    /// Subtracts an use of the item and deletes it from the players inventory if depleted.
    /// </summary>
    private void DecreaseAmount()
    {
        amount--;

        if (amount == 0)
        {
            inventory.RemoveItem(this);
        }
    }

    /// <summary>
    /// The function of the item. This is called each time OnUseItem is invoked.
    /// </summary>
    protected abstract void ItemFunction(Character user);

    /// <summary>
    /// Invokes OnUseItem to use the item.
    /// </summary>
    public void UseItem(Character user)
    {
        if (OnUseItem != null)
        {
            OnUseItem.Invoke(user);
        }
    }

    /// <summary>
    /// Creates a string of the name and the amount left of this item.
    /// </summary>
    public override string ToString()
    {
        return name + ": " + amount;
    }
}