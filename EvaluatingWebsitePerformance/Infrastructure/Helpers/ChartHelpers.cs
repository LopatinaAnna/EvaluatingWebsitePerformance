using EvaluatingWebsitePerformance.Data.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

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

            var chart = new Chart
            {
                Width = 1700,
                Height = 450,
                BackColor = Color.White,
                BorderlineDashStyle = ChartDashStyle.NotSet,
                BackSecondaryColor = Color.White,
                BackGradientStyle = GradientStyle.TopBottom,
                BorderlineWidth = 0,
                Palette = ChartColorPalette.SemiTransparent,
                BorderlineColor = Color.FromArgb(26, 59, 105),
                RenderType = RenderType.BinaryStreaming
            };

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