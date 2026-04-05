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

        int gridSize = 3;
        Random rand = new Random();

        List<string> roomNames = new List<string> { "Entrance", "Hallway", "Treasure", "Library", "Armory", "Chamber", "Vault", "Sanctum", "Cellar" };
        for (int i = roomNames.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            var temp = roomNames[i];
            roomNames[i] = roomNames[j];
            roomNames[j] = temp;
        }

        Room[,] grid = new Room[gridSize, gridSize];
        int nameIdx = 0;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                grid[x, y] = new Room(roomNames[nameIdx++], x, y);
                _rooms.Add(grid[x, y]);
            }
        }

        bool[,] visited = new bool[gridSize, gridSize];
        void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
        void DFS(int x, int y)
        {
            visited[x, y] = true;
            var directions = new List<(string dir, int dx, int dy, string opp)>
            {
                ("north", 0, -1, "south"),
                ("south", 0, 1, "north"),
                ("west", -1, 0, "east"),
                ("east", 1, 0, "west")
            };
            Shuffle(directions);
            foreach (var (dir, dx, dy, opp) in directions)
            {
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize && !visited[nx, ny])
                {
                    grid[x, y].AddConnection(dir, grid[nx, ny]);
                    grid[nx, ny].AddConnection(opp, grid[x, y]);
                    DFS(nx, ny);
                }
            }
        }
        DFS(0, 0);

        int extraConnections = rand.Next(2, 5);
        for (int i = 0; i < extraConnections; i++)
        {
            int x1 = rand.Next(gridSize);
            int y1 = rand.Next(gridSize);
            var room1 = grid[x1, y1];
            var possibleDirs = new List<(string dir, int dx, int dy, string opp)>
            {
                ("north", 0, -1, "south"),
                ("south", 0, 1, "north"),
                ("west", -1, 0, "east"),
                ("east", 1, 0, "west")
            };
            Shuffle(possibleDirs);
            foreach (var (dir, dx, dy, opp) in possibleDirs)
            {
                int x2 = x1 + dx, y2 = y1 + dy;
                if (x2 >= 0 && x2 < gridSize && y2 >= 0 && y2 < gridSize)
                {
                    var room2 = grid[x2, y2];
                    if (!room1.GetDirections().Contains(dir))
                    {
                        room1.AddConnection(dir, room2);
                        room2.AddConnection(opp, room1);
                        break;
                    }
                }
            }
        }

        // Item pool
        List<Item> items = new List<Item>
        {
            new Item("Key", "A small rusty key.", "Key"),
            new Item("Potion", "A red healing potion.", "Consumable"),
            new Item("Gold Coin", "A shiny gold coin.", "Treasure"),
            new Item("Sword", "A sharp sword.", "Weapon"),
            new Item("Shield", "A sturdy shield.", "Armor"),
        };

        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            var temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }

        foreach (var item in items)
        {
            int rx = rand.Next(gridSize);
            int ry = rand.Next(gridSize);
            grid[rx, ry].AddItem(item);
        }

        // Enemy pool
        List<Enemy> enemies = new List<Enemy>
        {
            new Enemy(Enemy.GenerateRandomName(), 30),
            new Enemy(Enemy.GenerateRandomName(), 25),
            new Enemy(Enemy.GenerateRandomName(), 40)
        };

        foreach (var enemy in enemies)
        {
            int rx = rand.Next(gridSize);
            int ry = rand.Next(gridSize);
            grid[rx, ry].AddEnemy(enemy);
        }

            // Code lock puzzle
            int numLocks = rand.Next(1, 3);
            HashSet<Room> lockedRooms = new HashSet<Room>();
            List<(Room lockedRoom, string code)> lockInfos = new List<(Room, string)>();
            while (lockedRooms.Count < numLocks)
            {
                int rx = rand.Next(gridSize);
                int ry = rand.Next(gridSize);
                var room = grid[rx, ry];
                if (room != _rooms[0] && !lockedRooms.Contains(room))
                {
                    // Generate a 3-digit code
                    string code = rand.Next(100, 1000).ToString();
                    room.SetLockCode(code);
                    lockedRooms.Add(room);
                    lockInfos.Add((room, code));
                }
            }

            // Note with the code
            foreach (var (lockedRoom, code) in lockInfos)
            {
                Room noteRoom;
                do {
                    int rx = rand.Next(gridSize);
                    int ry = rand.Next(gridSize);
                    noteRoom = grid[rx, ry];
                } while (lockedRooms.Contains(noteRoom));
                noteRoom.AddItem(new Item($"Note", $"A crumpled note. It reads: The code is {code}.", "Note"));
            }
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

            // Combat
            var enemies = currentRoom.GetEnemies();
            if (enemies.Count > 0)
            {
                Console.WriteLine("Combat begins! Enemies in the room:");
                foreach (var enemy in enemies)
                {
                    Console.WriteLine($"- {enemy.Name}: {enemy.GetHealth()} HP");
                }
                while (enemies.Count > 0 && _player.GetHealth() > 0)
                {
                    DrawMap(currentRoom);
                    if (enemies.Count > 0)
                    {
                        Console.WriteLine("Enemies in the room:");
                        foreach (var enemy in enemies)
                        {
                            Console.WriteLine($"- {enemy.Name}: {enemy.GetHealth()} HP");
                        }
                    }
                    Console.WriteLine("\nYour turn! Type 'attack <enemy>', 'use <item>', or 'quit':");
                    string combatChoice = Console.ReadLine() ?? "";
                    if (combatChoice.ToLower() == "quit")
                    {
                        Console.WriteLine("You have quit the game.");
                        return;
                    }
                    bool playerActed = false;
                    bool shieldBlock = false;
                    if (combatChoice.ToLower().StartsWith("attack "))
                    {
                        string enemyName = combatChoice.Substring(7).Trim();
                        string Normalize(string s) => string.Join(" ", s.ToLower().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries));
                        string normInput = Normalize(enemyName);
                        var target = enemies.Find(e => Normalize(e.Name) == normInput);
                        if (target != null)
                        {
                            int dmg = 10;
                            int before = target.GetHealth();
                            int after = before - dmg;
                            var healthField = typeof(Character).GetField("_health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (healthField != null)
                            {
                                healthField.SetValue(target, after);
                            }
                            int actualDmg = before - Math.Max(after, 0);
                            Console.WriteLine($"You attack the {target.Name} for {actualDmg} damage!");
                            if (after <= 0)
                            {
                                Console.WriteLine($"You defeated the {target.Name}!");
                                currentRoom.RemoveEnemy(target);
                                enemies.Remove(target);
                            }
                            playerActed = true;
                        }
                        else
                        {
                            Console.WriteLine($"No enemy named '{enemyName}' here.");
                        }
                    }
                    else if (combatChoice.ToLower().StartsWith("use "))
                    {
                        string itemName = combatChoice.Substring(4).Trim();
                        var invField = typeof(Player).GetField("_inventory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var inventory = invField != null ? invField.GetValue(_player) as List<Item> : null;
                        var item = inventory?.Find(i => i.GetName().ToLower() == itemName.ToLower());
                        if (item != null && item.GetItemType() == "Consumable")
                        {
                            int heal = 20;
                            Console.WriteLine($"You use {item.GetName()} and heal {heal} HP!");
                            var healthField = typeof(Character).GetField("_health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (healthField != null)
                            {
                                healthField.SetValue(_player, Math.Min(_player.GetHealth() + heal, 100));
                            }
                            if (inventory != null)
                            {
                                inventory.Remove(item);
                            }
                            playerActed = true;
                        }
                        else if (item != null && item.GetItemType() == "Armor" && item.GetName().ToLower() == "shield")
                        {
                            Console.WriteLine("You raise your shield and block all attacks this round!");
                            if (inventory != null)
                            {
                                inventory.Remove(item);
                            }
                            playerActed = true;
                            shieldBlock = true;
                        }
                        else if (item != null && item.GetItemType() == "Weapon" && item.GetName().ToLower() == "sword")
                        {
                            if (enemies.Count == 0)
                            {
                                Console.WriteLine("There are no enemies to use the sword on.");
                            }
                            else
                            {
                                Console.WriteLine("Which enemy do you want to slay instantly? Type the name:");
                                string targetName = Console.ReadLine() ?? "";
                                string Normalize(string s) => string.Join(" ", s.ToLower().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries));
                                string normInput = Normalize(targetName);
                                var target = enemies.Find(e => Normalize(e.Name) == normInput);
                                if (target != null)
                                {
                                    Console.WriteLine($"You swing your sword and instantly slay the {target.Name}!");
                                    var healthField = typeof(Character).GetField("_health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                    if (healthField != null)
                                    {
                                        healthField.SetValue(target, 0);
                                    }
                                    Console.WriteLine($"You defeated the {target.Name}!");
                                    currentRoom.RemoveEnemy(target);
                                    enemies.Remove(target);
                                    if (inventory != null)
                                    {
                                        inventory.Remove(item);
                                    }
                                    playerActed = true;
                                }
                                else
                                {
                                    Console.WriteLine($"No enemy named '{targetName}' here.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"You can't use that item now.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid action. Try 'attack <enemy>' or 'use <item>' or 'quit'.");
                        continue;
                    }
                    if (playerActed && !shieldBlock)
                    {
                        foreach (var enemy in new List<Enemy>(enemies))
                        {
                            if (enemy.GetHealth() > 0)
                            {
                                int edmg = 8;
                                int pBefore = _player.GetHealth();
                                int pAfter = pBefore - edmg;
                                var healthField = typeof(Character).GetField("_health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                if (healthField != null)
                                {
                                    healthField.SetValue(_player, pAfter);
                                }
                                int actualEDmg = pBefore - Math.Max(pAfter, 0);
                                Console.WriteLine($"The {enemy.Name} attacks you for {actualEDmg} damage!");
                                if (_player.GetHealth() <= 0)
                                {
                                    DrawMap(currentRoom);
                                    Console.WriteLine("You have died. Better luck next time!");
                                    return;
                                }
                            }
                        }
                    }
                }
                if (_player.GetHealth() > 0)
                {
                    Console.WriteLine("You are victorious! All enemies defeated.");
                }
                continue;
            }

            // Show items in the room
            var items = currentRoom.GetItems();
            if (items.Count > 0)
            {
                Console.WriteLine("You see the following items:");
                foreach (var item in items)
                {
                    Console.WriteLine($"- {item.GetName()}: {item.GetDescription()}");
                }
            }

            Console.WriteLine("Available directions:");
            foreach (string dir in currentRoom.GetDirections())
            {
                Console.WriteLine($"- {dir}");
            }

            Console.WriteLine("Type a direction, 'pickup <item>', 'drop <item>', 'examine <item>', or 'quit':");

            string choice = Console.ReadLine() ?? "";
            if (choice.ToLower().StartsWith("examine "))
            {
                string itemName = choice.Substring(8).Trim();
                var item = items.Find(i => i.GetName().ToLower() == itemName.ToLower());
                if (item != null)
                {
                    Console.WriteLine($"You examine the {item.GetName()}: {item.GetDescription()}");
                }
                else
                {
                    var invField = typeof(Player).GetField("_inventory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var inventory = invField != null ? invField.GetValue(_player) as List<Item> : null;
                    var invItem = inventory?.Find(i => i.GetName().ToLower() == itemName.ToLower());
                    if (invItem != null)
                    {
                        Console.WriteLine($"You examine the {invItem.GetName()}: {invItem.GetDescription()}");
                    }
                    else
                    {
                        Console.WriteLine($"No item named '{itemName}' here or in your inventory.");
                    }
                }
                continue;
            }

            if (choice.ToLower() == "quit")
                break;

            if (choice.ToLower() == "inventory")
            {
                _player.ShowInventory();
                continue;
            }

            if (choice.ToLower().StartsWith("pickup "))
            {
                string itemName = choice.Substring(7).Trim();
                var item = items.Find(i => i.GetName().ToLower() == itemName.ToLower());
                if (item != null)
                {
                    currentRoom.RemoveItem(item);
                    _player.AddItem(item);
                }
                else
                {
                    Console.WriteLine($"No item named '{itemName}' here.");
                }
                continue;
            }

            if (choice.ToLower().StartsWith("drop "))
            {
                string itemName = choice.Substring(5).Trim();
                var invField = typeof(Player).GetField("_inventory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var inventory = invField != null ? invField.GetValue(_player) as List<Item> : null;
                var item = inventory?.Find(i => i.GetName().ToLower() == itemName.ToLower());
                if (item != null)
                {
                    _player.RemoveItem(item);
                    currentRoom.AddItem(item);
                }
                else
                {
                    Console.WriteLine($"You don't have '{itemName}' in your inventory.");
                }
                continue;
            }

            Room? nextRoom = currentRoom.GetRoom(choice.ToLower());
                if (nextRoom != null)
                {
                    if (nextRoom.HasLock() && !nextRoom.IsLockSolved())
                    {
                        Console.WriteLine("This room is locked with a code lock! Enter the 3-digit code to unlock, or type 'back' to stay:");
                        string codeInput = Console.ReadLine() ?? "";
                        if (codeInput == nextRoom.GetLockCode())
                        {
                            nextRoom.SolveLock();
                            Console.WriteLine("The lock clicks open! You may enter.");
                            currentRoom = nextRoom;
                        }
                        else if (codeInput.ToLower() == "back")
                        {
                            Console.WriteLine("You stay in the current room.");
                        }
                        else
                        {
                            Console.WriteLine("Incorrect code. The room remains locked.");
                        }
                    }
                    else
                    {
                        currentRoom = nextRoom;
                    }
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
        int gridSize = 3;
        var invField = typeof(Player).GetField("_inventory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var inventory = invField != null ? invField.GetValue(_player) as List<Item> : null;
        List<string> invLines = new List<string>();
        invLines.Add("+---------------------+");
        invLines.Add("|     Inventory       |");
        invLines.Add("+---------------------+");
        if (inventory == null || inventory.Count == 0)
        {
            invLines.Add("| (empty)            |");
        }
        else
        {
            foreach (var item in inventory)
            {
                string name = item.GetName();
                if (name.Length > 19) name = name.Substring(0, 16) + "...";
                invLines.Add($"| {name.PadRight(19)}|");
            }
        }
        invLines.Add("+---------------------+");

        // Draw map and inventory
        int mapHeight = gridSize * 2 - 1;
        int invHeight = invLines.Count;
        int totalHeight = Math.Max(mapHeight, invHeight);
        List<string> mapLines = new List<string>();
        for (int y = 0; y < gridSize; y++)
        {
            string row = "";
            // Draw rooms row
            for (int x = 0; x < gridSize; x++)
            {
                Room? room = GetRoomAt(x, y);
                if (room != null && room.IsVisited())
                {
                    if (room == currentRoom)
                        row += "[P]";
                    else
                        row += "[O]";
                }
                else
                {
                    row += "[.]";
                }

                if (x < gridSize - 1)
                {
                    Room? eastRoom = GetRoomAt(x + 1, y);
                    if (room != null && eastRoom != null && room.GetDirections().Contains("east") && eastRoom.GetDirections().Contains("west"))
                        row += "-";
                    else
                        row += " ";
                }
            }
            mapLines.Add(row);

            if (y < gridSize - 1)
            {
                string connRow = "";
                for (int x = 0; x < gridSize; x++)
                {
                    Room? room = GetRoomAt(x, y);
                    Room? southRoom = GetRoomAt(x, y + 1);
                    if (room != null && southRoom != null && room.GetDirections().Contains("south") && southRoom.GetDirections().Contains("north"))
                        connRow += " | ";
                    else
                        connRow += "   ";
                    if (x < gridSize - 1)
                        connRow += " ";
                }
                mapLines.Add(connRow);
            }
        }
        // Health bar
        int maxHealth = 100;
        int playerHealth = _player.GetHealth();
        int barLength = 20;
        int filled = (int)Math.Round((playerHealth / (double)maxHealth) * barLength);
        string healthBar = "[" + new string('#', filled) + new string('-', barLength - filled) + $"] {playerHealth}/{maxHealth} HP";

        // Print map, inventory, and health bar
        int healthBarLine = Math.Max(invLines.Count / 2, 1);
        for (int i = 0; i < totalHeight; i++)
        {
            string mapPart = i < mapLines.Count ? mapLines[i] : new string(' ', mapLines[0].Length);
            string invPart = i < invLines.Count ? invLines[i] : "";
            string healthPart = (i == healthBarLine) ? healthBar : "";
            Console.WriteLine(mapPart.PadRight(20) + "     " + invPart.PadRight(24) + "     " + healthPart);
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