using EvaluatingWebsitePerformance.BusinessLogic.Interfaces;
using EvaluatingWebsitePerformance.Data;
using EvaluatingWebsitePerformance.Data.Entities;
using HtmlAgilityPack;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EvaluatingWebsitePerformance.BusinessLogic.Services
{
    public class Service : IService
    {
        private readonly ApplicationDbContext context;

        private const int ATTEMPT_COUNT = 2;

        private const int SITEMAP_URLS_COUNT = 10;

        public Service(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<BaseRequest> AddBaseRequest(string baseRequestUrl, string userId)
        {

            var item = new BaseRequest
            {
                BaseRequestUrl = baseRequestUrl,
                Creation = DateTime.Now,
                UserId = userId
            };

            context.BaseRequests.Add(item);

            await context.SaveChangesAsync();

            var userRequests = await GetBaseRequestByUser(userId);

            var requestByUrl = userRequests.Where(c => c.BaseRequestUrl == baseRequestUrl).Reverse().FirstOrDefault();

            var sitemapsList = GetSitemapRequests(baseRequestUrl, requestByUrl.Id);

            requestByUrl.SitemapRequests = sitemapsList;

            await context.SaveChangesAsync();

            return requestByUrl;

        }

        public void AddSitemapRequest(SitemapRequest sitemapRequest)
        {
            context.SitemapRequests.Add(sitemapRequest);
            context.SaveChanges();
        }

        public async Task<List<BaseRequest>> GetBaseRequestByUser(string userId)
            => await context.BaseRequests.Where(c => c.UserId == userId).ToListAsync();

        public BaseRequest GetBaseRequest(string userId, string baseRequestUrl)
            =>  context.BaseRequests.Where(c => c.UserId == userId && c.BaseRequestUrl == baseRequestUrl).Reverse().FirstOrDefault();

        public async Task<SitemapRequest> GetSitemapRequestByBaseRequestId(int baseRequestId)
            => await context.SitemapRequests.FirstOrDefaultAsync(c => c.BaseRequestId == baseRequestId);

        private List<SitemapRequest> GetSitemapRequests(string baseRequestUrl, int baseRequestId)
        {
            var sitemapUrls = GetSitemapUrls(baseRequestUrl);

            var sitemapRequests = new List<SitemapRequest>();

            foreach (var item in sitemapUrls)
            {
                var sitemapRequest = new SitemapRequest { SitemapRequestUrl = item };

                for (int i = 0; i < ATTEMPT_COUNT; i++)
                {
                    WebRequest request = WebRequest.Create(item);
                    Stopwatch timer = new Stopwatch();

                    timer.Start();
                    try
                    {
                        request.GetResponse();
                        timer.Stop();

                        double time = Math.Round(timer.Elapsed.TotalMilliseconds);

                        sitemapRequest.ResponseTimes.Add(time);
                    }
                    catch
                    {
                        i--;
                    }
                }

                sitemapRequest.BaseRequestId = baseRequestId;
                sitemapRequest.MinResponseTime = sitemapRequest.ResponseTimes.Min();
                sitemapRequest.MaxResponseTime = sitemapRequest.ResponseTimes.Max();
                
                AddSitemapRequest(sitemapRequest);
                sitemapRequests.Add(sitemapRequest);
            }


            return sitemapRequests;
        }

        private List<string> GetSitemapUrls(string baseRequestUrl)
        {
            List<string> urls = new List<string>{ baseRequestUrl };

            var htmlDocument = new HtmlWeb().Load(baseRequestUrl);

            foreach (HtmlNode htmlNode in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
            {
                string href = htmlNode.GetAttributeValue("href", string.Empty);

                if(href.StartsWith(baseRequestUrl) && !urls.Contains(href))
                    urls.Add(href);

                if (urls.Count == SITEMAP_URLS_COUNT) break;
            }

            return urls;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}