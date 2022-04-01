using System.Collections.Generic;
using System.Linq;
using DevCafe.Core.Repositories;
using FluentAssertions;
using Xunit;

namespace DevCafe.Core.UnitTests;

//Step 2 tests
public class BillingServiceTests
{
    [Fact]
    public void No_input_items_returns_no_cost()
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        sut.CalculateBill(Enumerable.Empty<string>()).Total.Should().Be(0);
    }
    
    [InlineData(0.5, 0, "Cola")]
    [InlineData(1, 0, "Coffee")]
    [InlineData(2, 0.2, "Cheese Sandwich")]
    [InlineData(4.5, 0.9, "Steak Sandwich")]
    [Theory]
    public void Single_item_bills_are_correct(decimal expectedItemTotal, decimal expectedServiceCharge, string item)
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        var bill = sut.CalculateBill(new[] { item });
        bill.ItemTotal.Should().Be(expectedItemTotal);
        bill.ServiceCharge.Should().Be(expectedServiceCharge);
    }
    
    [InlineData(1.5, 0, "Cola", "Coffee")]
    [InlineData(7.5, 1.5, "Coffee", "Cheese Sandwich", "Steak Sandwich")]
    [InlineData(3, 0.3, "Cheese Sandwich", "Coffee")]
    [InlineData(7, 1.4, "Steak Sandwich", "Cola", "Cheese Sandwich")]
    [Theory]
    public void Multiple_item_bills_are_correct(decimal expectedItemTotal, decimal expectedServiceCharge, params string[] items)
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        var bill = sut.CalculateBill(items);
        bill.ItemTotal.Should().Be(expectedItemTotal);
        bill.ServiceCharge.Should().Be(expectedServiceCharge);
    }
    
    [InlineData(2, 0, "Cola", "Cola", "Cola", "Cola")]
    [InlineData(3, 0, "Coffee", "Coffee", "Coffee")]
    [InlineData(5, 0.5, "Cheese Sandwich", "Coffee", "Cheese Sandwich")]
    [InlineData(12.5, 2.5, "Steak Sandwich", "Cola", "Cheese Sandwich", "Cola", "Cola", "Steak Sandwich")]
    [Theory]
    public void Multiples_of_same_item_on_bills_are_correct(decimal expectedItemTotal, decimal expectedServiceCharge, params string[] items)
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        var bill = sut.CalculateBill(items);
        bill.ItemTotal.Should().Be(expectedItemTotal);
        bill.ServiceCharge.Should().Be(expectedServiceCharge);
    }
    
    [InlineData("Tea")]
    [InlineData("Coffee", "Tea", "Beer")]
    [Theory]
    public void Unavailable_items_throws_exception(params string[] items)
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        var action = () => sut.CalculateBill(items);

        action.Should().Throw<UnavailableMenuItemException>();
    }
    
    [InlineData(0.5, 0, "CoLa")]
    [InlineData(1, 0, "COFFEE")]
    [InlineData(2, 0.2, "ChEeSe SaNdWiCh")]
    [InlineData(4.5, 0.9, "steak sandwich")]
    [Theory]
    public void Item_case_does_not_matter(decimal expectedItemTotal, decimal expectedServiceCharge, string item)
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        var bill = sut.CalculateBill(new[] { item });
        bill.ItemTotal.Should().Be(expectedItemTotal);
        bill.ServiceCharge.Should().Be(expectedServiceCharge);
    }
    
    [Fact]
    public void Order_containing_hot_food_service_charge_maxes_out_at_20()
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        var thatIsALotOfSteakSandwiches = new List<string>();

        for (var i = 0; i <= 25; i++)
        {
            thatIsALotOfSteakSandwiches.Add("Steak Sandwich");
        }
        
        thatIsALotOfSteakSandwiches.Add("Cola");
        
        var bill = sut.CalculateBill(thatIsALotOfSteakSandwiches);
        bill.ItemTotal.Should().Be(113);
        bill.ServiceCharge.Should().Be(20);
    }
    
    [Fact]
    public void Order_containing_no_hot_food_service_charge_can_exceed_20()
    {
        var sut = new BillingService(new InMemoryMenuItemRepository());

        var howCanAnyoneNeedThisManyCheeseSandwiches = new List<string>();

        for (var i = 0; i <= 100; i++)
        {
            howCanAnyoneNeedThisManyCheeseSandwiches.Add("Cheese Sandwich");
        }
        
        howCanAnyoneNeedThisManyCheeseSandwiches.Add("Coffee");
        
        var bill = sut.CalculateBill(howCanAnyoneNeedThisManyCheeseSandwiches);
        bill.ItemTotal.Should().Be(201);
        bill.ServiceCharge.Should().Be(20.1M);
    }
}