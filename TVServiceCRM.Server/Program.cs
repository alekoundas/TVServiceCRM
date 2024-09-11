using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using TVServiceCRM.Server.Business;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Business.Services;
using TVServiceCRM.Server.DataAccess;
using TVServiceCRM.Server.Model.Models;
using TVServiceCRM.Server.Model.System;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>() ?? default!;

builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        // Serialize enums as strings in api responses (e.g. Role).
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Ignore omitted parameters on models to enable optional params (e.g. User update).
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        // Ignore circular references.
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo() { Title = "App Api", Version = "v1" });
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    config.AddSecurityRequirement(
        new OpenApiSecurityRequirement{
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});



builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApiDbContext>();
builder.Services.AddScoped<ApiDbContextInitialiser>();
builder.Services.AddSingleton(TimeProvider.System);

// Add services.
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddSingleton(appSettings); // appsettings.json


builder.Services.AddDataProtection().PersistKeysToDbContext<ApiDbContext>();


// Add Identity.
builder.Services.AddIdentityCore<ApplicationUser>()
               .AddRoles<IdentityRole>()
               .AddSignInManager()
               .AddEntityFrameworkStores<ApiDbContext>()
               .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("REFRESHTOKENPROVIDER");

// Configure JWT Bearer token and refresh token.
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromSeconds(appSettings.RefreshTokenExpireSeconds);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        ValidAudiences = appSettings.Audiences,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey)),
        ClockSkew = TimeSpan.FromSeconds(0)
    };
});



var app = builder.Build();

// NGINX reverse proxy.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// CORS.
app.UseCors(options =>
    options
        .WithOrigins(appSettings.Audiences.ToArray())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");



// Run migrations to DB and add initial roles to DB.
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApiDbContextInitialiser>();
    await initialiser.RunMigrationsAsync();
    await initialiser.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // No HTTPS.
    //app.UseHttpsRedirection();
}

app.Run();
