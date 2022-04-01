namespace DevCafe.Core;

public class BillingService
{
    public Bill CalculateBill(IEnumerable<string> itemNames)
    {
        return new Bill(-1, -1);
    }
}