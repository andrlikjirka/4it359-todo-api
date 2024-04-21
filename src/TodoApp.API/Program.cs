using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoApp.Api.Configuration;
using TodoApp.Api.Data;
using TodoApp.Api.Extensions;
using TodoApp.Api.HostedServices;
using TodoApp.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);
builder.Services.Configure<TaskCollectorOptions>(builder.Configuration.GetSection("TaskCollector"));

builder.Services.AddControllers();
builder.Services
    .AddDbContext<ItemContext>(options =>
    {
        options.UseInMemoryDatabase(builder.Configuration.GetValue<string>("Database:Name") ?? "default");
    })
    .AddScoped<IItemRepository, ItemRepository>()
    .AddHostedService<TaskCollector>()
    .AddHostedService<TaskMarker>();

/*
 * if (builder.Configuration.Get<TaskCollectorOptions>().EnableTaskCollector)
   {
       builder.Services.AddHostedService<TaskCollector>();
   }
 */


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseMiddleware<RequestBodyLengthMiddleware>();
app.MapControllers();
app.SeedData();

app.Run(); 

public partial class Program;