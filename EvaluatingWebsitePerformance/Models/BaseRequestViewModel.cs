using EvaluatingWebsitePerformance.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EvaluatingWebsitePerformance.Models
{
    public class BaseRequestViewModel
    {
        public string BaseRequestUrl { get; set; }

        public DateTime Creation { get; set; }

        public List<SitemapRequest> SitemapRequests { get; set; }
    }
}