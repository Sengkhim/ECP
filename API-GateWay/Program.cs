using API_GateWay.core.extensions;
using Microsoft.EntityFrameworkCore;

// using Library.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.Services.AddServiceCollections(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCaching();

var app = builder.Build();

app.AddApplicationMiddlewares();
app.Run();