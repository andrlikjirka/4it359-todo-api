using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TodoApp.Api.Middleware;
using Microsoft.AspNetCore.Mvc;

public class RequestBodyLengthMiddleware
{
    private readonly RequestDelegate _next;

    public RequestBodyLengthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        //using var reader = new StreamReader(context.Request.Body);
        //var body = await reader.ReadToEndAsync();
        
        if (context.Request.ContentLength > 500)
        {
            context.Response.StatusCode = 413;
            var responseObject = new
            {
                error = $"The request body is too long. Allowed maximum is 500, but the request had {context.Request.ContentLength}.",
            };
            var response = JsonSerializer.Serialize(responseObject);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response);
            
            return;
        }
        
        await _next(context);
    }
}