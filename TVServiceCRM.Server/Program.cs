using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Business.Services;
using TVServiceCRM.Server.DataAccess;
using TVServiceCRM.Server.Model.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();







//builder.Services.AddLettuceEncrypt();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ApiDbContext>();
builder.Services.AddSingleton(TimeProvider.System);
// Add services.
builder.Services.AddScoped<IDataService, DataService>();



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


// Enable CORS.
//builder.Services.AddCors(c => c.AddPolicy("CorsPolicy", options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()));


// Add Identity.
//builder.Services.AddDataProtection().PersistKeysToDbContext<ApiDbContext>();
//builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
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
}

//app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//app.UseAuthorization();

app.MapControllers();
//app.MapIdentityApi<User>();
//app.UseCors("CorsPolicy");
app.UseCors(
        options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    ); 




app.MapFallbackToFile("/index.html");

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    var context = services.GetRequiredService<ApiDbContext>();
//    if (context.Database.GetPendingMigrations().Any())
//    {
//        context.Database.Migrate();
//    }
//}

app.Run();
