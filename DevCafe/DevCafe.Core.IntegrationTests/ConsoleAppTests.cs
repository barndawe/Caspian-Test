using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DevCafe.Core.IntegrationTests;

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
    [InlineData(2, "Cheese Sandwich")]
    [InlineData(4.5, "Steak Sandwich")]
    [Theory]
    public async Task Single_item_bills_are_correct(decimal expectedBill, string item)
    {
        var sut = StartCafeMenuConsoleApp(item);

        var response = await WaitForResponse(sut);

        response.Should().Be(expectedBill.ToString("0.00"));
    }
    
    [InlineData(1.5, "Cola", "Coffee")]
    [InlineData(7.5, "Coffee", "Cheese Sandwich", "Steak Sandwich")]
    [InlineData(3, "Cheese Sandwich", "Coffee")]
    [InlineData(7, "Steak Sandwich", "Cola", "Cheese Sandwich")]
    [Theory]
    public async Task Multiple_item_bills_are_correct(decimal expectedBill, params string[] items)
    {
        var sut = StartCafeMenuConsoleApp(items);

        var response = await WaitForResponse(sut);

        response.Should().Be(expectedBill.ToString("0.00"));
    }
    
    [InlineData(2, "Cola", "Cola", "Cola", "Cola")]
    [InlineData(3, "Coffee", "Coffee", "Coffee")]
    [InlineData(5, "Cheese Sandwich", "Coffee", "Cheese Sandwich")]
    [InlineData(12.5, "Steak Sandwich", "Cola", "Cheese Sandwich", "Cola", "Cola", "Steak Sandwich")]
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
    [InlineData(2, "ChEeSe SaNdWiCh")]
    [InlineData(4.5, "steak sandwich")]
    [Theory]
    public async Task Item_case_does_not_matter(decimal expectedBill, string item)
    {
        var sut = StartCafeMenuConsoleApp(item);

        var response = await WaitForResponse(sut);

        response.Should().Be(expectedBill.ToString("0.00"));
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