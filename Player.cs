using System;
using System.Collections.Generic;

public class Player : Character
{
    private List<Item> _inventory;

    public Player(string name, int health) : base(name, health)
    {
        _inventory = new List<Item>();
    }

    public override void Attack()
    {
        Console.WriteLine($"{_name} attacks!");
    }

    public void AddItem(Item item)
    {
        _inventory.Add(item);
        Console.WriteLine($"Added {item.GetName()} to inventory.");
    }

    public void RemoveItem(Item item)
    {
        if (_inventory.Contains(item))
        {
            _inventory.Remove(item);
            Console.WriteLine($"Removed {item.GetName()} from inventory.");
        }
        else
        {
            Console.WriteLine($"{item.GetName()} not found in inventory.");
        }
    }

    public void ShowInventory()
    {
        if (_inventory.Count == 0)
        {
            Console.WriteLine("Inventory is empty.");
            return;
        }
        Console.WriteLine("Inventory:");
        foreach (var item in _inventory)
        {
            Console.WriteLine($"- {item.GetName()}: {item.GetDescription()}");
        }
    }
}