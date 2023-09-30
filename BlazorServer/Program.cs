using Blazored.SessionStorage;
using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();

    /*builder.Services
        .AddServerSideBlazor()
        .AddHubOptions(opt =>
        {
            opt.DisableImplicitFromServicesParameters = true;
        });*/


    builder.Services.Configure<HubOptions>(options => { options.DisableImplicitFromServicesParameters = true; });


    builder.WebHost.ConfigureKestrel(opciones =>
    { 
        opciones.Limits.MaxRequestBodySize = 512 * 1024 * 1024;
    });

    builder.Services.Configure<FormOptions>(opciones =>
    {
        // Limite de 512 Mbps
        opciones.MultipartBodyLengthLimit = 512 * 1024 * 1024;
    });

    // Declarar los servicios a utilizar
    builder.Services.AddHttpClient<IServicioAlumnos, ServicioAlumnos>(cliente =>
    {
        cliente.BaseAddress = new Uri(builder.Configuration["RutaApi"]!);
    });

    builder.Services.AddHttpClient<IServicioCursos, ServicioCursos>(cliente =>
    {
	    cliente.BaseAddress = new Uri(builder.Configuration["RutaApi"]!);
    });

    builder.Services.AddHttpClient<IServicioLogin, ServicioLogin>(cliente =>
    {
        cliente.BaseAddress = new Uri(builder.Configuration["RutaApi"]!);
    });
    
    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddBlazoredSessionStorage();
    builder.Services.AddScoped<AuthenticationStateProvider, MiServicioAuthenticationStateProvider>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");

        app.UseHsts();
    }


    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();

}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}