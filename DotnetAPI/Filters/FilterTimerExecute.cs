using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace DotnetAPI.Filters
{
    public class FilterTimerExecute : ActionFilterAttribute
    {
        private Stopwatch _timer;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _timer = Stopwatch.StartNew();   
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _timer.Stop();
            var miliSeconds = _timer.ElapsedMilliseconds;
            var message = $"Action took {miliSeconds} ms to execute.";
            Console.WriteLine(message);
        }
    }
}
