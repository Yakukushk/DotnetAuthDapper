using Microsoft.AspNetCore.Mvc.Filters;

namespace DotnetAPI.Filters
{
    public class FilterAction : IActionFilter
    {
        bool State;
        public void OnActionExecuted(ActionExecutedContext context) // after
        {
            if (!context.HttpContext.Response.HasStarted)
            {
                context.HttpContext.Response.Headers.Append("AuthenticationState", State.ToString());
            }
        }


        public void OnActionExecuting(ActionExecutingContext context) // before
        {
            var auth = context.HttpContext.Request.Headers.Authorization;

            if(auth.Count != 0)
            {
                context.HttpContext.Request.Headers["AuthenticationState"] = "true";
                State = true;
            } else
            {
                context.HttpContext.Request.Headers["AuthenticationState"] = "false";
                State = false;
            }

            Console.WriteLine($"Status context : {auth}");
        }
    }
}
