namespace DevCafe.Core;

public class UnavailableMenuItemException : Exception
{
    public UnavailableMenuItemException(IEnumerable<string> unavailableItems) : base("Sorry, we don't serve that here.")
    {
        UnavailableItems = unavailableItems.ToList();
    }

    public IEnumerable<string> UnavailableItems { get;}
}