using MachineManagement.ManagerApp.Services;
using MudBlazor.Services;
using Serilog;
using AutoMapper;
using MachineManagement.ManagerApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add HttpClient for API calls
builder.Services.AddHttpClient("ApiClient", client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add custom services
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<ICommandService, CommandService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Add SignalR
builder.Services.AddSignalR();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Add SignalR hubs
app.MapHub<Hubs.MachineHub>("/machinehub");
app.MapHub<Hubs.LogHub>("/loghub");

app.Run();