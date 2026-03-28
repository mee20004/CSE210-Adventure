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
    }
}