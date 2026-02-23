using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Questly.Exceptions;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is null) return;

        
        if (context.Exception is KeyNotFoundException notFoundException)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound,
                Detail = notFoundException.Message,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status404NotFound
            };
            context.ExceptionHandled = true;
        }
        else if (context.Exception is ArgumentException validationException)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Invalid input parameters",
                Status = StatusCodes.Status400BadRequest,
                Detail = validationException.Message,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            context.ExceptionHandled = true;
        } 
        else if (context.Exception is UnauthorizedAccessException unauthorizedException)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Invalid input parameters",
                Status = StatusCodes.Status401Unauthorized,
                Detail = unauthorizedException.Message,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            context.ExceptionHandled = true;
        } 
        else
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal server error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred. Please try again later.",
                Instance = context.HttpContext.Request.Path
            };

#if DEBUG
            problemDetails.Extensions["error_details"] = context.Exception.Message;
            problemDetails.Extensions["stack_trace"] = context.Exception.StackTrace;
#endif

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}