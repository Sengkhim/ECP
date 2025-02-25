using Library.Core.UnitOfWork;
using Library.Persistent;
using Library.Persistent.Entities;
using Library.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Database;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDatabaseLayer<UserContext>(builder.Configuration);
var connectionString = builder.Configuration.GetConnectionString("ECP_DATABASE");

builder.Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<UserEntity, RoleEntity>()
    .AddEntityFrameworkStores<UserContext>()
    .AddDefaultTokenProviders();

// builder.Services.AddScoped<IIdentityDatabaseService, EcpIdentityDb<DbContext>>();
// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();