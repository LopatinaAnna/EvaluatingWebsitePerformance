using System.Web.Mvc;
using EvaluatingWebsitePerformance.BusinessLogic.Interfaces;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using EvaluatingWebsitePerformance.Data.Entities;
using EvaluatingWebsitePerformance.Models;
using System.Linq;

namespace EvaluatingWebsitePerformance.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService baseRequestService;

        public HomeController(IService baseRequestService)
        {
            this.baseRequestService = baseRequestService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MeasureResponseTime(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return RedirectToAction("Index");
            }

            BaseRequest item = await baseRequestService.AddBaseRequest(url, User.Identity.GetUserId());

            ViewData["url"] = url;

            var resultModel = new BaseRequestViewModel
            {
                BaseRequestUrl = item.BaseRequestUrl,
                Creation = item.Creation,
                SitemapRequests = item.SitemapRequests
                .OrderBy(c => c.MinResponseTime)
                .ToList()
            };

            return MeasureResponseTime(resultModel);
        }

        [HttpGet]
        public ActionResult MeasureResponseTime(BaseRequestViewModel model)
        {
            return View(model);
        }
    }
}