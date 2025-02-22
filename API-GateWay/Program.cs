using API_GateWay.core.extensions;
using API_GateWay.Infrastructure.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.Services.AddEpcDatabase(builder.Configuration);
builder.Services.AddServiceCollections(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCaching();

var app = builder.Build();

app.AddApplicationMiddlewares();
app.Run();

