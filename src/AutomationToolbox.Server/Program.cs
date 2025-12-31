using AutomationToolbox.Core.Interfaces;
using AutomationToolbox.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register Scanner Services
builder.Services.AddSingleton<INetworkProbe, RealNetworkProbe>();
builder.Services.AddScoped<IScannerService, ScannerService>();

// Register Modbus Server Manager
builder.Services.AddSingleton<IModbusServerManager, ModbusServerManager>();

// Allow CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}

// app.UseHttpsRedirection(); // Disable HTTPS redirection to simplify localhost usage as per user request

app.UseCors("AllowAll");

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
