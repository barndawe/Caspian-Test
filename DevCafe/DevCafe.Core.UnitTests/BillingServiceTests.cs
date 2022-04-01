using System.Linq;
using FluentAssertions;
using Xunit;

namespace DevCafe.Core.UnitTests;

//Step 1 tests
public class BillingServiceTests
{
    [Fact]
    public void No_input_items_returns_no_cost()
    {
        var sut = new BillingService();

        sut.CalculateBill(Enumerable.Empty<string>()).ItemTotal.Should().Be(0);
    }
    
    [InlineData(0.5, "Cola")]
    [InlineData(1, "Coffee")]
    [InlineData(2, "Cheese Sandwich")]
    [InlineData(4.5, "Steak Sandwich")]
    [Theory]
    public void Single_item_bills_are_correct(decimal expectedBill, string item)
    {
        var sut = new BillingService();

        sut.CalculateBill(new[] { item }).ItemTotal.Should().Be(expectedBill);
    }
    
    [InlineData(1.5, "Cola", "Coffee")]
    [InlineData(7, "Coffee", "Cheese Sandwich", "Steak Sandwich")]
    [InlineData(3, "Cheese Sandwich", "Coffee")]
    [InlineData(7, "Steak Sandwich", "Cola", "Cheese Sandwich")]
    [Theory]
    public void Multiple_item_bills_are_correct(decimal expectedBill, params string[] items)
    {
        var sut = new BillingService();

        sut.CalculateBill(items).ItemTotal.Should().Be(expectedBill);
    }
    
    [InlineData(2, "Cola", "Cola", "Cola", "Cola")]
    [InlineData(1, "Coffee", "Coffee", "Coffee")]
    [InlineData(5, "Cheese Sandwich", "Coffee", "Cheese Sandwich")]
    [InlineData(12.5, "Steak Sandwich", "Cola", "Cheese Sandwich", "Cola", "Cola", "Steak Sandwich")]
    [Theory]
    public void Multiples_of_same_item_on_bills_are_correct(decimal expectedBill, params string[] items)
    {
        var sut = new BillingService();

        sut.CalculateBill(items).ItemTotal.Should().Be(expectedBill);
    }
    
    //incorrect item(s) throws exception (theory)
    [InlineData("Tea")]
    [InlineData("Coffee", "Tea", "Beer")]
    [Theory]
    public void Unavailable_items_throws_exception(params string[] items)
    {
        var sut = new BillingService();

        var action = () => sut.CalculateBill(items);

        action.Should().Throw<UnavailableMenuItemException>();
    }
}