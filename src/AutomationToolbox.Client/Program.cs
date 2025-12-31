using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AutomationToolbox.Client;
using AutomationToolbox.Core.Interfaces;
using AutomationToolbox.Core.Services;
using AutomationToolbox.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register the HttpClient with the base address of the host environment.
// This is necessary for making requests to the server (if hosting APIs there) or external static files.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Register the IToolService dependency.
// Using MockToolService for now to provide static data without a backend API.
builder.Services.AddScoped<IToolService, MockToolService>();
builder.Services.AddScoped<IModbusServerClientService, ModbusServerClientService>();

var host = builder.Build();

// Read culture from local storage
var culture = new System.Globalization.CultureInfo("en-US");
try
{
    var js = host.Services.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
    var result = await js.InvokeAsync<string>("localStorage.getItem", new object[] { "culture" });

    if (!string.IsNullOrEmpty(result))
    {
        culture = new System.Globalization.CultureInfo(result);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading culture: {ex.Message}");
    // Fallback to default
}

System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
