using CalcSaMD.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// This is the one line that makes EnvironmentBadge.razor's call to
// Http.GetFromJsonAsync("env-config.json") resolve to *this app's own site*
// instead of needing a real backend address. builder.HostEnvironment.BaseAddress
// is filled in by the browser at load time — on your machine while testing
// locally it will be something like "https://localhost:5001/", and once
// published to Azure Static Web Apps (Phase 6) it will automatically become
// your Static Web App's own URL (or your custom domain, once bound). You
// never hard-code an address here, and that's deliberate: the same compiled
// output runs unmodified in DEV, QA, UAT and PROD.
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
