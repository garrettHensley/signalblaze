using Microsoft.AspNetCore.Components.Server.Circuits;
using SignalBlaze;
using SignalBlaze.Components;
using SignalBlaze.Hubs;
using SignalBlaze.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IMessageService, MessageService>();

builder.Services.AddSignalR(options => {
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.EnableDetailedErrors = true; // Helpful for debugging Railway logs
});

builder.Services.AddSingleton<ChatPresence>();
builder.Services.AddSingleton<CircuitHandler, ChatCircuitHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<ChatHub>("/chathub");

app.Run();
