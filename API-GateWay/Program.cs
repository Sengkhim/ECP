using API_GateWay.core.extensions;
using ECPLibrary.Extensions;
using ECPLibrary.Persistent;
using ECPLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.Services.AddServiceCollections(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCaching();

builder.Services.AddCoreEcpLibrary<EcpDatabase>((service) =>
{
    service.AddDbContext<EcpDatabase>(options => 
        options.UseNpgsql(builder.Configuration.GetConnectionString("ECP_DATABASE")));
        
    service.AddScoped<IEcpDatabase, EcpDatabase>();
        
    service.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<EcpDatabase>()
        .AddDefaultTokenProviders();
});

var app = builder.Build();

app.AddApplicationMiddlewares();
app.Run();