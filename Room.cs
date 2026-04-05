using System.Collections.Generic;

public class Room
{
    private string _name;
    private Dictionary<string, Room> _connections;
    private int _x;
    private int _y;
    private bool _visited;
    private List<Item> _items;
    private List<Enemy> _enemies;

// Puzzle: code lock
    private string? _lockCode;
    private bool _isLocked;
    private bool _isSolved;

    public Room(string name, int x, int y)
    {
        _name = name;
        _connections = new Dictionary<string, Room>();
        _x = x;
        _y = y;
        _visited = false;
        _items = new List<Item>();
        _enemies = new List<Enemy>();
        _lockCode = null;
        _isLocked = false;
        _isSolved = false;
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void SetLockCode(string code)
    {
        _lockCode = code;
        _isLocked = true;
        _isSolved = false;
    }

    public bool HasLock() => _isLocked;
    public bool IsLockSolved() => _isSolved;
    public string? GetLockCode() => _lockCode;
    public void SolveLock() { _isSolved = true; _isLocked = false; }

    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }

    public List<Enemy> GetEnemies()
    {
        return _enemies;
    }
    public void AddItem(Item item)
    {
        _items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        _items.Remove(item);
    }

    public List<Item> GetItems()
    {
        return _items;
    }

    public string GetName() => _name;

    public void AddConnection(string direction, Room room)
    {
        _connections[direction] = room;
    }

    public Room? GetRoom(string direction)
    {
        return _connections.ContainsKey(direction) ? _connections[direction] : null;
    }

    public List<string> GetDirections()
    {
        return new List<string>(_connections.Keys);
    }

    public int GetX() => _x;
    public int GetY() => _y;

    public bool IsVisited() => _visited;
    public void Visit() => _visited = true;
}