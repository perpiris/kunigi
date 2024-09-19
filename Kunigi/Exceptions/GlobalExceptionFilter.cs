using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kunigi.Exceptions;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext filterContext)
    {
        if (!filterContext.ExceptionHandled)
        {
            _logger.LogError(filterContext.Exception, 
                "An unhandled exception occurred in {Controller}/{Action}",
                filterContext.RouteData.Values["controller"],
                filterContext.RouteData.Values["action"]);
            
            filterContext.ExceptionHandled = true;
            
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary 
                {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
        }
    }
}