using System.Diagnostics.CodeAnalysis;
using TodoApp.Api.Data;
using TodoApp.Api.Helpers;

namespace TodoApp.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    [ExcludeFromCodeCoverage]
    public static void SeedData(this IApplicationBuilder app)
    {
       using var scope = app.ApplicationServices.CreateScope();
       var context = scope.ServiceProvider.GetRequiredService<ItemContext>();

       context.Items.AddRange(ItemGenerator.GenerateItems());
       context.SaveChanges();
    }
}