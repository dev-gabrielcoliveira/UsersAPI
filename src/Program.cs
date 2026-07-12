using FCG.Users.Application.Interfaces.Repository;
using FCG.Users.Application.Interfaces.Services;
using FCG.Users.Application.Services;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Configurando o EntityFramework, para se comunicar com o banco de dados e realizar as migrations
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsuariosConnection"),
        sqlServerOptions => sqlServerOptions.MigrationsHistoryTable("migrations_usuarios"));
});

string key = builder.Configuration["Jwt:Key"] ?? "";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => { policy.RequireRole("Administrador"); });
    options.AddPolicy("Usuario", policy => { policy.RequireRole("Usuario"); });
    options.AddPolicy("AdministradorOuUsuario", policy => { policy.RequireRole("Administrador", "Usuario"); });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Api de Usuários", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Digite 'Bearer {seu_token}' para autenticação"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

});

// DI
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, TokenService>();
builder.Services.AddScoped<UsuarioService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddMassTransit(busRegistration =>
//{
//    busRegistration.UsingRabbitMq((context, cfg) => {

//        //var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "rabbitmq-service";

//        //cfg.Host(rabbitHost, "/", hostConfigurator =>
//        cfg.Host("localhost", "/", hostConfigurator => 
//        {
//            hostConfigurator.Username("guest");
//            hostConfigurator.Password("guest");
//        });

//        cfg.ConfigureEndpoints(context);
//    });
//});

builder.Services.AddMassTransit(busRegistration =>
{
    busRegistration.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", hostConfigurator =>
        {
            hostConfigurator.Username("guest");
            hostConfigurator.Password("guest");
        });

        cfg.UseRawJsonSerializer();
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao rodar migrations: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
