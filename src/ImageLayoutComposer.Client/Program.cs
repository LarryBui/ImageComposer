using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ImageLayoutComposer.Client;
using ImageLayoutComposer.Client.Services;
using ImageLayoutComposer.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Assuming API is at this address for local development. In production, this would be the same host or from config.
var apiAddress = AppConstants.DefaultApiAddress;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiAddress) });
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();
