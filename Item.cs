public class Item
{
    private string _name;
    private string _description;
    private string _type;

    public Item(string name, string description, string type)
    {
        _name = name;
        _description = description;
        _type = type;
    }

    public string GetName() => _name;
    public string GetDescription() => _description;
    public string GetItemType() => _type;
}