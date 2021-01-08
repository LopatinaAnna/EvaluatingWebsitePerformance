using System.Web.Mvc;

namespace EvaluatingWebsitePerformance.Infrastructure.Filters
{
    public class ActionException : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext exceptionContext)
        {
            if (!exceptionContext.ExceptionHandled)
            {
                exceptionContext.Result = new RedirectResult("/Home/Index");
                exceptionContext.ExceptionHandled = true;
            }
        }
    }
}