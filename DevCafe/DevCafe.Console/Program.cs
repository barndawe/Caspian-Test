// See https://aka.ms/new-console-template for more information

using DevCafe.Core;
using DevCafe.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMenuItemRepository, InMemoryMenuItemRepository>();
        services.AddSingleton<BillingService>();
    }).Build();

var billingService = host.Services.GetService<BillingService>();

if (!args.Any())
{
    Console.WriteLine("Please pick something from the menu");
    return 1;
}

try
{
    var bill = billingService.CalculateBill(args);

    Console.WriteLine(bill.Total.ToString("0.00"));
    return 0;
}
catch (UnavailableMenuItemException unEx)
{
    Console.WriteLine(unEx.ToString());
    return 1;
}


