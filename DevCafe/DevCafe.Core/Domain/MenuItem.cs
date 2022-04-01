namespace DevCafe.Core.Domain;

public class MenuItem
{
    public MenuItem(string name, ItemCategory category, ItemTemperature temperature, decimal unitCost)
    {
        Name = name;
        Category = category;
        Temperature = temperature;
        UnitCost = unitCost;
    }

    public string Name { get; private set; }

    public ItemCategory Category { get; private set; }

    public ItemTemperature Temperature { get; private set; }

    public decimal UnitCost { get; private set; }
}