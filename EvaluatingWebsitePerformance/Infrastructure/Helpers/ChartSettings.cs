using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace EvaluatingWebsitePerformance.Infrastructure.Helpers
{
    public static class ChartSettings
    {
        public static Title CreateTitle()
        {
            Title title = new Title
            {
                Text = "",
                ShadowColor = Color.Black,
                Font = new Font("Arial", 16F, FontStyle.Bold),
                ShadowOffset = 0,
                ForeColor = Color.Black
            };

            return title;
        }

        public static Series CreateSeries(string[] names, double[] values, string seriesName, SeriesChartType chartType, Color color)
        {
            var seriesDetail = new Series
            {
                Name = seriesName,
                IsValueShownAsLabel = false,
                Color = color,
                ChartType = chartType,
                BorderWidth = 1,
                Font = new Font(new FontFamily("Arial"), 16)
            };

            seriesDetail["DrawingStyle"] = "Default";
            seriesDetail["PieDrawingStyle"] = "Default";
            DataPoint point;

            for (int i = 0; i < names.Length; i++)
            {
                point = new DataPoint
                {
                    AxisLabel = names[i],
                    YValues = new double[] { values[i] },
                    Font = new Font(new FontFamily("Arial"), 16)
                };
                seriesDetail.Points.Add(point);
            }

            return seriesDetail;
        }

        public static Legend CreateLegend()
        {
            var legend = new Legend
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                BackColor = Color.Transparent,
                Font = new Font(new FontFamily("Arial"), 12),
                LegendStyle = LegendStyle.Row
            };
            return legend;
        }

        public static ChartArea CreateChartArea()
        {
            var chartArea = new ChartArea
            {
                Name = "",
                BackColor = Color.Transparent
            };

            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.MajorTickMark.Enabled = false;
            chartArea.AxisY.MajorTickMark.Enabled = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 12F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 12F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.Transparent;
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.Transparent;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Title = "Response time, ms";
            chartArea.AxisY.TitleFont = new Font("Arial,sans-serif", 12F, FontStyle.Bold);

            return chartArea;
        }
    }
}