using System;
using System.Collections.Generic;

namespace EvaluatingWebsitePerformance.Data.Entities
{
    public class BaseRequest : BaseEntity
    {
        public string BaseRequestUrl { get; set; } 

        public List<SitemapRequest> SitemapRequests { get; set; } = new List<SitemapRequest>();

        public DateTime Creation { get; set; }

        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}