using Library.Extensions;
using UserService.Extensions;
using UserService.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
var development = app.Environment.IsDevelopment();
if (development)
{
    development.ApplyMigrate(app);
    app.UseSwaggerOptions();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();