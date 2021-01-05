using System.Diagnostics;
using System.Net;
using System.Web.Mvc;
using System.Web.Helpers;

namespace EvaluatingWebsitePerformance.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MeasureResponseTime(string url)
        {
            WebRequest request = WebRequest.Create(url);

            Stopwatch timer = new Stopwatch();

            double time;

            timer.Start();
            request.GetResponse();
            timer.Stop();

            time = timer.Elapsed.TotalSeconds;
            ViewData["url"] = url;
            return MeasureResponseTime(time);
        }

        [HttpGet]
        public ActionResult MeasureResponseTime(double? time)
        {
            return View(System.Math.Round(time.GetValueOrDefault(), 3));
        }

        //public ActionResult CreateChart()
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