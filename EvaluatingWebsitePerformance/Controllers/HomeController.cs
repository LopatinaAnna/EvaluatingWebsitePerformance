using System.Diagnostics;
using System.Net;
using System.Web.Mvc;
using System.Web.Helpers;
using EvaluatingWebsitePerformance.BusinessLogic.Interfaces;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using EvaluatingWebsitePerformance.Data.Entities;
using EvaluatingWebsitePerformance.Models;

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
            if(string.IsNullOrEmpty(url))
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
            };
            return MeasureResponseTime(resultModel);
        }

        [HttpGet]
        public ActionResult MeasureResponseTime(BaseRequestViewModel model)
        {
            return View(model);
        }

        //public ActionResult CreateChart(
        //{

        //    var chart = new Chart(width: 500, height: 200)
        //          .AddTitle("График посещений")
        //          .AddSeries(
        //                 name: "Моя программа",
        //                 legend: "Моя программа",
        //                 chartType: "Line",
        //                 xValue: new[] { "Peter", "Andrew", "Julie", "Mary", "Dave" },
        //                 yValues: new[] { "2", "6", "4", "5", "3" });

        //    return File(chart.ToWebImage().GetBytes(), "image/jpeg");
        //}
    }
}