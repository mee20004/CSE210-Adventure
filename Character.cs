public class Character
{
    protected string _name;
    public string Name => _name;
    protected int _health;

    public Character(string name, int health)
    {
        _name = name;
        _health = health;
    }

    public virtual void Attack()
    {

    }

    public int GetHealth()
    {
        return _health;
    }
}