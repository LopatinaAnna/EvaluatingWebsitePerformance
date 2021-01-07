using System;
using System.Collections.Generic;
using System.Linq;
using EvaluatingWebsitePerformance.Data.Entities;
using System.Web.Mvc;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using System.IO;

namespace EvaluatingWebsitePerformance.Infrastructure.Helpers
{
    public static class ChartHelpers
    {
        public static MvcHtmlString CreateChart(this HtmlHelper html,
            List<SitemapRequest> sitemapRequests)
        {
            var orderRequests = sitemapRequests
                .OrderBy(c => c.MinResponseTime);

            var namesArr = Enumerable
                .Range(1, sitemapRequests.Count)
                .Select(c => c.ToString())
                .ToArray();

            var minValuesArr = orderRequests
                .Select(c => c.MinResponseTime)
                .ToArray();
            var maxValuesArr = orderRequests
                .Select(c => c.MaxResponseTime)
                .ToArray();

            var chart = new Chart();
            chart.Width = 1700;
            chart.Height = 450;
            chart.BackColor = Color.White;
            chart.BorderlineDashStyle = ChartDashStyle.NotSet;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 0;
            chart.Palette = ChartColorPalette.SemiTransparent;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.None;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(ChartSettings.CreateTitle());
            chart.Legends.Add(ChartSettings.CreateLegend());
            chart.Series.Add(ChartSettings.CreateSeries(namesArr, minValuesArr, "Min response", SeriesChartType.Column, Color.LightGreen));
            chart.Series.Add(ChartSettings.CreateSeries(namesArr, maxValuesArr, "Max response", SeriesChartType.Column, Color.DarkGray));
            chart.ChartAreas.Add(ChartSettings.CreateChartArea());
            var ms = new MemoryStream();
            chart.SaveImage(ms);

            var base64Image = Convert.ToBase64String(ms.GetBuffer());

            TagBuilder tag = new TagBuilder("img");
            tag.MergeAttribute("src", $"data:image/png;base64,{base64Image}");

            return MvcHtmlString.Create(tag.ToString());
        }
    }
}