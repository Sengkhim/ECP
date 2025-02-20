using API_GateWay.core.extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddServiceCollections(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddTelemetryServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFixedWindowRateLimiter();
builder.Services.AddResponseCaching();
builder.AddLogging();
builder.Services.AddConsulClient();

var app = builder.Build();

app.AddApplicationMiddlewares();
app.Run();