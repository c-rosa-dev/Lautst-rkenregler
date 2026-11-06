using Laermampel;
using Laermampel.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Dependency Injection – wie providers in Angular
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<MediaService>();
await builder.Build().RunAsync();
