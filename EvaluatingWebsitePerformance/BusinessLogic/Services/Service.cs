using EvaluatingWebsitePerformance.BusinessLogic.Interfaces;
using EvaluatingWebsitePerformance.Data;
using EvaluatingWebsitePerformance.Data.Entities;
using EvaluatingWebsitePerformance.Infrastructure;
using HtmlAgilityPack;
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

        private const int ATTEMPT_COUNT = 3;

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

            var userRequests = await GetBaseRequestsByUser(userId);

            var requestByUrl = userRequests
                .Where(c => c.BaseRequestUrl == baseRequestUrl)
                .Reverse()
                .FirstOrDefault();

            List<SitemapRequest> sitemapsList;

            try
            {
                sitemapsList = await GetSitemapRequests(baseRequestUrl, requestByUrl.Id);
            }
            catch (ValidationException exception)
            {
                throw exception;
            }

            requestByUrl.SitemapRequests = sitemapsList;

            await context.SaveChangesAsync();

            return requestByUrl;
        }

        public async Task AddSitemapRequest(SitemapRequest sitemapRequest)
        {
            context.SitemapRequests.Add(sitemapRequest);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBaseRequest(string userId, string baseRequestUrl, DateTime creation)
        {
            var item = await context.BaseRequests
              .FirstOrDefaultAsync(c => c.UserId == userId 
              && c.BaseRequestUrl == baseRequestUrl 
              && c.Creation.Second == creation.Second);

            if (item != null)
            {
                context.BaseRequests.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllBaseRequest(string userId)
        {
            var item = await context.BaseRequests
              .Where(c => c.UserId == userId).ToListAsync();

            if (item != null)
            {
                context.BaseRequests.RemoveRange(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<BaseRequest>> GetBaseRequestsByUser(string userId)
            => await context.BaseRequests
            .Where(c => c.UserId == userId)
            .ToListAsync();

        public async Task<int> GetBaseRequestId(string userId, string baseRequestUrl, DateTime creation)
        {
           var item = await context.BaseRequests
              .FirstOrDefaultAsync(c => c.UserId == userId 
              && c.BaseRequestUrl == baseRequestUrl 
              && c.Creation.Second == creation.Second);

            return item.Id;
        }
        public async Task<SitemapRequest> GetSitemapRequestByBaseRequestId(int baseRequestId)
            => await context.SitemapRequests
            .FirstOrDefaultAsync(c => c.BaseRequestId == baseRequestId);


        public async Task<BaseRequest> GetBaseRequests(int id)
         => await context.BaseRequests
            .Include(c =>c.SitemapRequests)
            .FirstOrDefaultAsync(c => c.Id == id);

        private async Task<List<SitemapRequest>> GetSitemapRequests(string baseRequestUrl, int baseRequestId)
        {
            List<string> sitemapUrls;
            try
            {
                sitemapUrls = await GetSitemapUrls(baseRequestUrl);
            }
            catch (ValidationException exception)
            {
                throw exception;
            }

            var sitemapRequests = new List<SitemapRequest>();

            foreach (var item in sitemapUrls)
            {
                var sitemapRequest = new SitemapRequest 
                { 
                    SitemapRequestUrl = item, 
                    BaseRequestId = baseRequestId 
                };

                for (int i = 0; i < ATTEMPT_COUNT; i++)
                {
                    double time = await GetRequestTime(item);

                    sitemapRequest.ResponseTimes.Add(time);
                }

                if (sitemapRequest.ResponseTimes.Count >= 2)
                {
                    sitemapRequest.MinResponseTime = sitemapRequest.ResponseTimes.Min();
                    sitemapRequest.MaxResponseTime = sitemapRequest.ResponseTimes.Max();

                    await AddSitemapRequest(sitemapRequest);
                    sitemapRequests.Add(sitemapRequest);
                }
            }
            return sitemapRequests;
        }

        private async Task<double> GetRequestTime(string item)
        {
            WebRequest request = WebRequest.Create(item);
            Stopwatch timer = new Stopwatch();

            timer.Start();
            await request.GetResponseAsync();
            timer.Stop();

            return Math.Round(timer.Elapsed.TotalMilliseconds);
        }

        private async Task<List<string>> GetSitemapUrls(string baseRequestUrl)
        {
            List<string> urls = new List<string> { baseRequestUrl };

            HtmlNodeCollection nodes;
            try
            {
                var htmlDocument = await new HtmlWeb().LoadFromWebAsync(baseRequestUrl);
                nodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            }
            catch (Exception)
            {
                throw new ValidationException("Invalid html document");
            }

            if(nodes == null || nodes.Count == 0)
            {
                throw new ValidationException("Invalid html document");
            }

            foreach (var htmlNode in nodes)
            {
                string href = htmlNode.GetAttributeValue("href", string.Empty);

                if (href.StartsWith(baseRequestUrl) && !urls.Contains(href))
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