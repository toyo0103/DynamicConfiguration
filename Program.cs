using Amazon.DynamoDBv2;
using DynamicConfigLab.DynamicConfiguration;
using DynamicConfigLab.DynamicConfiguration.PollingMode;
using DynamicConfigLab.DynamicConfiguration.PollingMode.Interfaces;
using DynamicConfigLab.Models;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);
builder.AddDynamicConfiguration();

var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IConfigurationRepository,ConfigurationRepository>();

builder.Services.Configure<FeatureSettings>(
    builder.Configuration.GetSection("FeatureSettings")
);
builder.Services.Configure<AppSettings>(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog((ctx, svc, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(svc)
        .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
);

builder.Services.AddHostedService<ConfigPollingService>();

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