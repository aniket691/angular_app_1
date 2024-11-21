using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ValidateModelAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if the model state is invalid
        if (!context.ModelState.IsValid)
        {
            // Return a 400 Bad Request response with validation errors
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // You can handle post-action logic here if needed
    }
}
