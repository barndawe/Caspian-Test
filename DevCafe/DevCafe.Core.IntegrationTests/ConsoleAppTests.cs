using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DevCafe.Core.IntegrationTests;

//Step 2 tests
public class ConsoleAppTests
{
    [Fact]
    public async Task No_input_items_returns_error_message()
    {
        var sut = StartCafeMenuConsoleApp();

        var response = await WaitForResponse(sut);

        response.Should().Be("Please pick something from the menu");
    }
    
    [InlineData(0.5, "Cola")]
    [InlineData(1, "Coffee")]
    [InlineData(2.2, "Cheese Sandwich")]
    [InlineData(5.4, "Steak Sandwich")]
    [Theory]
    public async Task Single_item_bills_are_correct(decimal expectedBill, string item)
    {
        var sut = StartCafeMenuConsoleApp(item);

        var response = await WaitForResponse(sut);

        response.Should().Be(expectedBill.ToString("0.00"));
    }
    
    [InlineData(1.5, "Cola", "Coffee")]
    [InlineData(9, "Coffee", "Cheese Sandwich", "Steak Sandwich")]
    [InlineData(3.3, "Cheese Sandwich", "Coffee")]
    [InlineData(8.4, "Steak Sandwich", "Cola", "Cheese Sandwich")]
    [Theory]
    public async Task Multiple_item_bills_are_correct(decimal expectedBill, params string[] items)
    {
        var sut = StartCafeMenuConsoleApp(items);

        var response = await WaitForResponse(sut);

        response.Should().Be(expectedBill.ToString("0.00"));
    }
    
    [InlineData(2, "Cola", "Cola", "Cola", "Cola")]
    [InlineData(3, "Coffee", "Coffee", "Coffee")]
    [InlineData(5.5, "Cheese Sandwich", "Coffee", "Cheese Sandwich")]
    [InlineData(15, "Steak Sandwich", "Cola", "Cheese Sandwich", "Cola", "Cola", "Steak Sandwich")]
    [Theory]
    public async Task Multiples_of_same_item_on_bills_are_correct(decimal expectedBill, params string[] items)
    {
        var sut = StartCafeMenuConsoleApp(items);

        var response = await WaitForResponse(sut);

        response.Should().Be(expectedBill.ToString("0.00"));
    }
    
    [InlineData("tea", "Tea")]
    [InlineData("tea, beer", "Coffee", "Tea", "Beer")]
    [Theory]
    public async Task Unavailable_items_throws_exception(string unavailableItems, params string[] items)
    {
        var sut = StartCafeMenuConsoleApp(items);

        var response = await WaitForResponse(sut);

        response.Should().Be($"Sorry, we don't serve that here. Items: {unavailableItems}");
    }
    
    [InlineData(0.5, "CoLa")]
    [InlineData(1, "COFFEE")]
    [InlineData(2.2, "ChEeSe SaNdWiCh")]
    [InlineData(5.4, "steak sandwich")]
    [Theory]
    public async Task Item_case_does_not_matter(decimal expectedBill, string item)
    {
        var sut = StartCafeMenuConsoleApp(item);

        var response = await WaitForResponse(sut);

        response.Should().Be(expectedBill.ToString("0.00"));
    }
    
    [Fact]
    public async Task Order_containing_hot_food_service_charge_maxes_out_at_20()
    {
        var thatIsALotOfSteakSandwiches = new List<string>();

        for (var i = 0; i <= 25; i++)
        {
            thatIsALotOfSteakSandwiches.Add("Steak Sandwich");
        }
        
        thatIsALotOfSteakSandwiches.Add("Cola");
        
        var sut = StartCafeMenuConsoleApp(thatIsALotOfSteakSandwiches.ToArray());

        var response = await WaitForResponse(sut);

        response.Should().Be(133.ToString("0.00"));
    }
    
    [Fact]
    public async Task Order_containing_no_hot_food_service_charge_can_exceed_20()
    {
        var howCanAnyoneNeedThisManyCheeseSandwiches = new List<string>();

        for (var i = 0; i <= 100; i++)
        {
            howCanAnyoneNeedThisManyCheeseSandwiches.Add("Cheese Sandwich");
        }
        
        howCanAnyoneNeedThisManyCheeseSandwiches.Add("Coffee");
        
        var sut = StartCafeMenuConsoleApp(howCanAnyoneNeedThisManyCheeseSandwiches.ToArray());

        var response = await WaitForResponse(sut);

        response.Should().Be(221.1M.ToString("0.00"));
    }
    
    private Process StartCafeMenuConsoleApp(params string[] arguments)
    {
        //a little bit brittle but I've never written test for a console app before!
        var solutionDirectory = FindSolutionDirectory();
        var consoleAppDirectory = Path.Combine(solutionDirectory, @"DevCafe.Console\bin\Debug\net6.0");
        
        
        var processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = Path.Combine(consoleAppDirectory, "DevCafe.Console.exe");
        processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        processStartInfo.RedirectStandardInput = true;
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.Arguments = string.Join(" ", arguments.Select(a => $"\"{a}\""));

        return Process.Start(processStartInfo);
    }
    
    private Task<string?> WaitForResponse(Process process)
    {
        return Task.Run(() =>
        {
            var output = process.StandardOutput.ReadLine();

            return output;
        });
    }

    private string FindSolutionDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }
        return directory.FullName;
    }
}