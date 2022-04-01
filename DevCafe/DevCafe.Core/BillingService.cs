using DevCafe.Core.Domain;
using DevCafe.Core.Repositories;

namespace DevCafe.Core;

public class BillingService
{
    private readonly IMenuItemRepository _itemRepository;

    public BillingService(IMenuItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }
    
    public Bill CalculateBill(IEnumerable<string> itemNames)
    {
        var menuItemsAndCounts = GetValidMenuItemsAndCounts(itemNames).ToList();

        var itemTotal = CalculateItemTotal(menuItemsAndCounts);
        var serviceCharge = CalculateServiceCharge(menuItemsAndCounts.Select(mc => mc.item), itemTotal);

        return new Bill(itemTotal, serviceCharge);
    }

    private IEnumerable<(MenuItem item, int count)> GetValidMenuItemsAndCounts(IEnumerable<string> itemNames)
    {
        var itemNameList = itemNames.Select(i => i.ToLowerInvariant()).ToList();
        
        var distinctItemNames = itemNameList.Distinct().ToList();

        var menuItems = _itemRepository.GetItemsByNames(distinctItemNames)
            .ToDictionary(i => i.Name, i => i, StringComparer.OrdinalIgnoreCase);

        var unknownItemNames = distinctItemNames.Where(i => !menuItems.ContainsKey(i))
            .ToList();

        if (unknownItemNames.Any())
        {
            throw new UnavailableMenuItemException(unknownItemNames);
        }

        foreach (var itemName in distinctItemNames)
        {
            yield return (
                menuItems[itemName],
                itemNameList.Count(i => i == itemName));
        }
    }

    private decimal CalculateItemTotal(IEnumerable<(MenuItem, int)> menuItemsAndCounts)
    {
        return menuItemsAndCounts.Aggregate(0M, (total, itemAndCount) =>
        {
            var (menuItem, count) = itemAndCount;
            return total + (menuItem.UnitCost * count);
        });
    }

    private decimal CalculateServiceCharge(IEnumerable<MenuItem> orderedMenuItems, decimal itemTotal)
    {
        var menuItems = orderedMenuItems.ToList();
        if (menuItems.Any(i => i.Category == ItemCategory.Food && i.Temperature == ItemTemperature.Hot))
        {
            var serviceCharge = itemTotal * 0.2M;

            if (serviceCharge > 20)
            {
                serviceCharge = 20;
            }

            return serviceCharge;
        }

        if (menuItems.Any(i => i.Category == ItemCategory.Food))
        {
            //The spec did not explicitly say to cap the service charge at 20 for cold food-only orders
            return itemTotal * 0.1M;
        }

        return 0;
    }
}