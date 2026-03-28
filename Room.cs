using System.Collections.Generic;

public class Room
{
    private string _name;
    private Dictionary<string, Room> _connections;
    private int _x;
    private int _y;
    private bool _visited;

    public Room(string name, int x, int y)
    {
        _name = name;
        _connections = new Dictionary<string, Room>();
        _x = x;
        _y = y;
        _visited = false;
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