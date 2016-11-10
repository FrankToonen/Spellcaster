using System;
using UnityEngine;

[Serializable]
public abstract class Item
{
    private event Action OnUseItem;

    public string name;
    public int amount;

    protected Item(string name, int amount)
    {
        this.name = name;
        this.amount = amount;

        OnUseItem += DecreaseAmount;
        OnUseItem += ItemFunction;
    }

    private void DecreaseAmount()
    {
        amount--;

        if (amount == 0)
        {
            GameObject.FindWithTag("Player").GetComponent<Player>().RemoveItem(this);
        }
    }

    protected abstract void ItemFunction();

    public void UseItem()
    {
        if (OnUseItem != null)
        {
            OnUseItem.Invoke();
        }
    }

    public override string ToString()
    {
        return name + ": " + amount;
    }
}