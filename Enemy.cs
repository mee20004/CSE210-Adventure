using System;

public class Enemy : Character
{
    private static Random _rand = new Random();

    private static string[] _adjectives = {
        "Blorpy", "Zindle", "Snazzle", "Gloopy", "Wibby", "Plonky", "Frizzle", "Morbly", "Twizzle", "Grimbly",
        "Jibber", "Sproingy", "Vroopy", "Drimpy", "Fuzzle", "Wompy", "Zonky", "Brizzle", "Skrunkly", "Plimsy"
    };
    private static string[] _nouns = {
        "Goblin", "Skeleton", "Orc", "Zombie", "Troll", "Witch", "Beast", "Bandit", "Slime", "Imp",
        "Snorf", "Gribble", "Womp", "Plink", "Zorp", "Fuzzle", "Dribble", "Splug", "Mog", "Twimp"
    };

    public static string GenerateRandomName()
    {
        string adj = _adjectives[_rand.Next(_adjectives.Length)];
        string noun = _nouns[_rand.Next(_nouns.Length)];
        return $"{adj} {noun}";
    }

    public Enemy(string name, int health) : base(name, health)
    {
    }

    public override void Attack()
    {
        Console.WriteLine($"{_name} attacks!");
    }
}