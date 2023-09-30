using APIAlumnos.Datos;
using APIAlumnos.Repositorio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{

	var builder = WebApplication.CreateBuilder(args);

	// Add services to the container.

	builder.Services.AddControllers();
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen(c =>
	{
		c.EnableAnnotations();

		c.SwaggerDoc("v1",
			new OpenApiInfo
			{
				Title = "API Alumnos",
				Version = "v1",
				Description = "",
				TermsOfService = new Uri("http://tempuri.org/terms"),
				Contact = new OpenApiContact
				{
					Name = "Miguel Martínez",
					Email = "mmartinez@mmartinezdev.com"
				},
				License = new OpenApiLicense
				{
					Name = "Apache 2.0",
					Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
				}
			}
		);
		var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
		c.IncludeXmlComments(xmlPath);

		/*
		c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
		{
			Name = "Authorization",
			Type = SecuritySchemeType.ApiKey,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
		});
		c.AddSecurityRequirement(new OpenApiSecurityRequirement {
			{
				new OpenApiSecurityScheme {
					Reference = new OpenApiReference {
						Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
					}
				},
				new string[] {}
			}
		});*/
		//c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
		string[] methodsOrder = new string[] { "get", "post", "put", "patch", "delete", "options", "trace" };
		c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{Array.IndexOf(methodsOrder, apiDesc.HttpMethod!.ToLower())}");
	});

	// Registra la interfaz e implementación del repositorio
	builder.Services.AddScoped<IRepositorioAlumnos, RepositorioAlumnos>();
	builder.Services.AddScoped<IRepositorioCursos, RepositorioCursos>();

	// Configuracion de conexión para MSSQL
	var cadenaConexionSqlConfiguracion = new AccesoDatos(builder.Configuration.GetConnectionString("SQL")!);
	builder.Services.AddSingleton(cadenaConexionSqlConfiguracion);

	// JWT
	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["JWT:Issuer"],
			ValidAudience = builder.Configuration["JWT:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["JWT:ClaveSecreta"])
				)
        };
	});

	builder.Services.AddMvc(option => option.EnableEndpointRouting = false);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();

        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

	app.UseAuthorization();

	app.MapControllers();

	// JWT se agrega para la autenticación
	app.UseAuthentication();
	app.UseMvc();

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
