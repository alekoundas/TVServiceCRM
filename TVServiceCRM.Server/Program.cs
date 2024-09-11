using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TVServiceCRM.Server.Business;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Business.Services;
using TVServiceCRM.Server.DataAccess;
using TVServiceCRM.Server.Model.Models;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSwaggerGen();



builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApiDbContext>();
builder.Services.AddScoped<ApiDbContextInitialiser>();
builder.Services.AddSingleton(TimeProvider.System);

// Add services.
builder.Services.AddScoped<IDataService, DataService>();
//builder.Services.ConfigureHttpJsonOptions(x => x.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));












// Add Identity.
builder.Services.AddDataProtection().PersistKeysToDbContext<ApiDbContext>();
//builder.Services.AddAuthorization();
//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
//        options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
//        //options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//    })
//    .AddCookie("Identity.Bearer", options =>
//    {
//         options.AccessDeniedPath = "/Auth/AccessDenied";
//         options.LoginPath = "/api/users/Login";
//     });



//var key = Encoding.ASCII.GetBytes("asdasdasdasdaasdasaSDASDasdasDasdASdAsDASDAsdAsDaSDAsdasDASDasDASdAsDas");

//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(x =>
//{
//    x.RequireHttpsMetadata = false;
//    x.SaveToken = true;
//    x.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = false,
//        ValidateAudience = false
//    };
//});
//var appSettings = builder.Configuration.GetSection("TokenSettings").Get() ?? default!;
//builder.Services.AddSingleton(appSettings);


builder.Services.AddIdentityCore<ApplicationUser>()
             .AddRoles<IdentityRole>()
             .AddSignInManager()
             .AddEntityFrameworkStores<ApiDbContext>();
             //.AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("REFRESHTOKENPROVIDER");

//builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
//{
//    options.TokenLifespan = TimeSpan.FromHours(12);
//});

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
        ValidAudiences = new[] { "http://localhost:8080", "https://localhost:8080", "http://localhost:5173", "https://localhost:5173" },
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DFDGERsjsfjepoeoe@@#$$@$@123112sdaaadasQEWw")),
        //ClockSkew = TimeSpan.FromSeconds(0)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Retrieve token from cookies
            var accessToken = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});





//builder.Services.Configure<IdentityOptions>(options =>
//{
//    // Default Password settings.
//    options.Password.RequireDigit = false;
//    options.Password.RequireLowercase = false;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireUppercase = false;
//    options.Password.RequiredLength = 5;
//    options.Password.RequiredUniqueChars = 0;
//});
//.AddCookie(IdentityConstants.ApplicationScheme);
//builder.Services
//    .AddIdentityCore<User>(options =>
//    {
//        // Default Password settings.
//        options.Password.RequireDigit = false;
//        options.Password.RequireLowercase = false;
//        options.Password.RequireNonAlphanumeric = false;
//        options.Password.RequireUppercase = false;
//        options.Password.RequiredLength = 5;
//        options.Password.RequiredUniqueChars = 0;
//        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-._@+";
//        options.User.RequireUniqueEmail = true;
//    })
//    .AddEntityFrameworkStores<ApiDbContext>()
//    .AddDefaultTokenProviders()
//    .AddApiEndpoints();









var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// NGINX reverse proxy.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseCors(options => options.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://alexps.gr", "http://alexps.gr").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");



//app.MapGroup("/api/users").MapIdentityApi<User>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // No HTTPS.
    //app.UseHttpsRedirection();
}




// Run migrations to DB and add initial roles to DB.
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApiDbContextInitialiser>();
    await initialiser.RunMigrationsAsync();
    await initialiser.SeedAsync();
}

app.Run();
