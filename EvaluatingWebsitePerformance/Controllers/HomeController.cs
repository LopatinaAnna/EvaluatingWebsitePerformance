using System.Web.Mvc;

namespace EvaluatingWebsitePerformance.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}