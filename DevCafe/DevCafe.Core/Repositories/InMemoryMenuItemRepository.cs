using DevCafe.Core.Domain;

namespace DevCafe.Core.Repositories;

public class InMemoryMenuItemRepository : IMenuItemRepository
{
    private static IEnumerable<MenuItem> Items = new[]
    {
        new MenuItem("Cola", ItemCategory.Drink, ItemTemperature.Cold, 0.5M),
        new MenuItem("Coffee", ItemCategory.Drink, ItemTemperature.Hot, 1M),
        new MenuItem("Cheese Sandwich", ItemCategory.Food, ItemTemperature.Cold, 2M),
        new MenuItem("Steak Sandwich", ItemCategory.Food, ItemTemperature.Hot, 4.5M)
    };

    public IEnumerable<MenuItem> GetItemsByNames(IEnumerable<string> itemNames)
    {
        return Items.Where(i => itemNames.Contains(i.Name, StringComparer.InvariantCultureIgnoreCase));
    }
}