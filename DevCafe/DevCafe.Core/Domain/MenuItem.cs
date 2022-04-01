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

    public string Name { get; }

    public ItemCategory Category { get; }

    public ItemTemperature Temperature { get; }

    public decimal UnitCost { get; }
}