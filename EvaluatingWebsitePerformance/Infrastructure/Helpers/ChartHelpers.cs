using System;
using System.Collections.Generic;
using System.Linq;
using EvaluatingWebsitePerformance.Data.Entities;
using System.Web.Helpers;
using System.Web.Mvc;

namespace EvaluatingWebsitePerformance.Infrastructure.Helpers
{
    public static class ChartHelpers
    {
        public static MvcHtmlString CreateChart(this HtmlHelper html,
            List<SitemapRequest> sitemapRequests)
        {
            var orderRequests = sitemapRequests
                .OrderBy(c => c.MinResponseTime);

            var namesArr = sitemapRequests
                .Select(c => c.SitemapRequestUrl)
                .ToArray();
            var minValuesArr = sitemapRequests
                .Select(c => c.MinResponseTime.ToString())
                .ToArray();
            var maxValuesArr = sitemapRequests
                .Select(c => c.MaxResponseTime.ToString())
                .ToArray();

            var chart = new Chart(width: 1100, height: 400)
                  .AddTitle("Website performance")
                  .SetYAxis(title: "Evaluating, ms")
                  .AddSeries(
                         name: "Min response",
                         chartType: "Column",
                         xValue: namesArr,
                         yValues: minValuesArr,
                         axisLabel: "")
                  .AddSeries(
                         name: "",
                         chartType: "Column",
                         xValue: namesArr,
                         yValues: maxValuesArr);

            var base64Image = Convert.ToBase64String(chart.ToWebImage().GetBytes());

            TagBuilder tag = new TagBuilder("img");
            tag.MergeAttribute("src", $"data:image/png;base64,{base64Image}");

            return MvcHtmlString.Create(tag.ToString());
        }
    }
}