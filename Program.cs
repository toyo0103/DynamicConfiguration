using DynamicConfigLab;
using DynamicConfigLab.DynamicConfiguration;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddDynamicConfiguration();

builder.Services.Configure<FeatureSettings>(
    builder.Configuration.GetSection("FeatureSettings")
);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog((ctx, svc, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(svc)
);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.Run();