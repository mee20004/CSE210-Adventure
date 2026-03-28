using System;
using System.Collections.Generic;

public class Game
{
    private Player _player;
    private List<Room> _rooms;

    public Game()
    {
        _player = new Player("Hero", 100);
        _rooms = new List<Room>();

        Room entrance = new Room("Entrance", 0, 0);
        Room hallway = new Room("Hallway", 0, 1);
        Room treasure = new Room("Treasure", 1, 1);

        entrance.AddConnection("north", hallway);

        hallway.AddConnection("south", entrance);
        hallway.AddConnection("east", treasure);

        treasure.AddConnection("west", hallway);

        _rooms.Add(entrance);
        _rooms.Add(hallway);
        _rooms.Add(treasure);
    }

    public void Start()
    {
        Console.WriteLine("Welcome to the Adventure Game!");

        Room currentRoom = _rooms[0];

        while (true)
        {
            currentRoom.Visit();

            DrawMap(currentRoom);

            Console.WriteLine($"\nYou are in: {currentRoom.GetName()}");

            Console.WriteLine("Available directions:");
            foreach (string dir in currentRoom.GetDirections())
            {
                Console.WriteLine($"- {dir}");
            }

            Console.WriteLine("Type a direction or 'quit':");
            string choice = Console.ReadLine() ?? "";

            if (choice.ToLower() == "quit")
                break;

            Room? nextRoom = currentRoom.GetRoom(choice.ToLower());

            if (nextRoom != null)
            {
                currentRoom = nextRoom;
            }
            else
            {
                Console.WriteLine("You can't go that way.");
            }
        }
    }

    private void DrawMap(Room currentRoom)
    {
        Console.WriteLine("\nMap:");

        for (int y = 2; y >= -1; y--)
        {
            for (int x = -1; x <= 2; x++)
            {
                Room? room = GetRoomAt(x, y);

                if (room != null && room.IsVisited())
                {
                    if (room == currentRoom)
                        Console.Write("P "); // Player
                    else
                        Console.Write("O "); // Visited room
                }
                else
                {
                    Console.Write(". ");
                }
            }
            Console.WriteLine();
        }
    }

    private Room? GetRoomAt(int x, int y)
    {
        foreach (Room room in _rooms)
        {
            if (room.GetX() == x && room.GetY() == y)
                return room;
        }
        return null;
    }
}