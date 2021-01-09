﻿namespace EvaluatingWebsitePerformance.Models
{
    public class CreateBaseRequestModel
    {
        public string BaseRequestUrl { get; set; }

        public string UserId { get; set; }

        public int UrlsCount { get; set; }

        public int AttemptCount { get; set; }
    }
}