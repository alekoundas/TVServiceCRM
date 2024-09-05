using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Business.Services;
using TVServiceCRM.Server.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
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
builder.Services.AddSingleton(TimeProvider.System);
// Add services.
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.ConfigureHttpJsonOptions(x => x.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));



//builder.Services.AddDataProtection()
//    .SetApplicationName("KEKW")
//    //.PersistKeysToFileSystem(new DirectoryInfo(@"C:\temp-keys\"))
//    .AddKeyManagementOptions(options =>
//    {
//        options.NewKeyLifetime = new TimeSpan(365, 0, 0, 0);
//        options.AutoGenerateKeys = true;
//    })
//    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
//    {
//        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
//        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
//    });



// Add Identity.
//builder.Services.AddDataProtection().PersistKeysToDbContext<ApiDbContext>();
//builder.Services.AddAuthorization();
// builder.Services.AddAuthentication();
//.AddCookie(IdentityConstants.ApplicationScheme);
//builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<ApiDbContext>().AddApiEndpoints();









var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthorization();

app.MapControllers();
//app.MapIdentityApi<User>();
//app.UseCors("CorsPolicy");
app.UseCors(
        options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    );




app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiDbContext>();

    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}

app.Run();
