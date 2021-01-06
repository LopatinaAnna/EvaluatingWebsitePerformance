using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace EvaluatingWebsitePerformance.Infrastructure.Helpers
{
    public static class ChartSettings
    {
        public static Title CreateTitle()
        {
            Title title = new Title();
            title.Text = "";
            title.ShadowColor = Color.Black;
            title.Font = new Font("Arial", 16F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.Black;

            return title;
        }

        public static Series CreateSeries(string[] names, double[] values, string seriesName, SeriesChartType chartType, Color color)
        {
            var seriesDetail = new Series();
            seriesDetail.Name = seriesName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = color;
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 1;
            seriesDetail.Font = new Font(new FontFamily("Arial"), 16);
            seriesDetail["DrawingStyle"] = "Default";
            seriesDetail["PieDrawingStyle"] = "Default";
            DataPoint point;

            for (int i = 0; i < names.Length; i++)
            {
                point = new DataPoint();
                point.AxisLabel = names[i];
                point.YValues = new double[] { values[i] };
                point.Font = new Font(new FontFamily("Arial"), 16);
                seriesDetail.Points.Add(point);
            }

            return seriesDetail;
        }

        public static Legend CreateLegend()
        {
            var legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Arial"), 12);
            legend.LegendStyle = LegendStyle.Row;
            return legend;
        }

        public static ChartArea CreateChartArea()
        {
            var chartArea = new ChartArea();
            chartArea.Name = "";
            chartArea.BackColor = Color.Transparent;
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