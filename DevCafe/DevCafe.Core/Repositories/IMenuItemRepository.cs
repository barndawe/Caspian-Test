using DevCafe.Core.Domain;

namespace DevCafe.Core.Repositories;

public interface IMenuItemRepository
{
    IEnumerable<MenuItem> GetItemsByNames(IEnumerable<string> itemNames);
}