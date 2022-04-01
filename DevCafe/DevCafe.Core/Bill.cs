namespace DevCafe.Core;

public record Bill(decimal ItemTotal, decimal ServiceCharge)
{
    public decimal Total => ItemTotal + ServiceCharge;
}