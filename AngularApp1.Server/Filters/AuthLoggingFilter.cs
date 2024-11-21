using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

public class AuthLoggingFilter : IActionFilter
{
    private readonly ILogger<AuthLoggingFilter> _logger;

    public AuthLoggingFilter(ILogger<AuthLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Logic before action execution if needed
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.ActionDescriptor.DisplayName.Contains("Login") && context.Result is OkObjectResult)
        {
            var result = (OkObjectResult)context.Result;
            _logger.LogInformation("User logged in successfully. Result: {Result}", result.Value);
        }
        else if (context.ActionDescriptor.DisplayName.Contains("Register") && context.Result is OkResult)
        {
            _logger.LogInformation("User registration completed successfully.");
        }
        // Add more conditions as needed for other actions
    }
}
