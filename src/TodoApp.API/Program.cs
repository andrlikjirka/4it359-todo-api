﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TodoApp.Api.Configuration;
using TodoApp.Api.Data;
using TodoApp.Api.Extensions;
using TodoApp.Api.HostedServices;
using TodoApp.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Security:TokenSecret"))),
            IssuerSigningKeyResolver = (_, _, _, parameters) => new[] { parameters.IssuerSigningKey },
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddOptions<TaskCollectorOptions>()
    .Bind(builder.Configuration.GetSection("TaskCollector"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddControllers();
builder.Services
    .AddDbContext<ItemContext>(options =>
    {
        options.UseInMemoryDatabase(builder.Configuration.GetValue<string>("Database:Name") ?? "default");
    })
    .AddScoped<IItemRepository, ItemRepository>()
    .AddHostedService<TaskMarker>();

if (builder.Configuration.GetValue<bool>("TaskCollector:EnableTaskCollector"))
{
    builder.Services.AddHostedService<TaskCollector>();
}

// logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options => { options.IncludeScopes = true; });
if (builder.Configuration.GetValue<bool>("Observability:Enabled"))
{
    builder.Logging.AddFilter("TodoApp.Api", LogLevel.Information);
    /* Alternative: Open Telemetry Logging
    builder.Logging.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("TodoApp"));
        options.AddConsoleExporter();
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
    });*/
    // Open Telemetry Tracing
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("TodoApp"))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter());
}
// end: logging

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseMiddleware<RequestBodyLengthMiddleware>();
app.MapControllers();
app.SeedData();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program;