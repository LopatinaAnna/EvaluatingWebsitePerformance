using System.Web.Mvc;

namespace EvaluatingWebsitePerformance.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult HTTP404()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult HTTP413()
        {
            Response.StatusCode = 413;
            return View();
        }

        public ActionResult HTTP400()
        {
            Response.StatusCode = 400;
            return View();
        }

        public ActionResult HTTP500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}