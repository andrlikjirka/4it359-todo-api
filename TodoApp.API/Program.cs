using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

builder.Services.AddControllers();
builder.Services
    .AddDbContext<ItemContext>(options =>
    {
        options.UseInMemoryDatabase(builder.Configuration.GetValue<string>("Database:Name") ?? "default");
    })
    .AddScoped<IItemRepository, ItemRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();
app.SeedData();

app.Run();