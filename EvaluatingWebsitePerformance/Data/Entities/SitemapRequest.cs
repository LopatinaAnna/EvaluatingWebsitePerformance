using System.Collections.Generic;

namespace EvaluatingWebsitePerformance.Data.Entities
{
    public class SitemapRequest : BaseEntity
    {
        public string SitemapRequestUrl { get; set; }

        public List<double> ResponseTimes { get; set; } = new List<double>();

        public double MaxResponseTime { get; set; }

        public double MinResponseTime { get; set; }

        public int BaseRequestId { get; set; }

        public BaseRequest Request { get; set; }
    }
}