using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace TodoApp.Api.Filters;

/// <summary>
/// Validates the model state of the request.
/// </summary>
internal class ModelValidationAttribute : ActionFilterAttribute, IFilterFactory
{
    private IOptionsMonitor<ApiBehaviorOptions> _optionsMonitor;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (_optionsMonitor?.CurrentValue.SuppressModelStateInvalidFilter ?? false)
        {
            return;
        }

        var modelState = context.ModelState;
        if (!modelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(modelState);
        }
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return  new ModelValidationAttribute
        {
            _optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<ApiBehaviorOptions>>()
        };
    }

    public bool IsReusable => true;
}