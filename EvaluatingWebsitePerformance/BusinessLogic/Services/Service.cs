using EvaluatingWebsitePerformance.BusinessLogic.Interfaces;
using EvaluatingWebsitePerformance.Data;
using EvaluatingWebsitePerformance.Data.Entities;
using EvaluatingWebsitePerformance.Infrastructure;
using EvaluatingWebsitePerformance.Models;
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

        private int ATTEMPT_COUNT = 2;

        private int SITEMAP_URLS_COUNT = 10;

        public Service(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<BaseRequest> AddBaseRequest(CreateBaseRequestModel model)
        {
            if(model.UrlsCount >= 1 && model.UrlsCount <= 30)
            {
                SITEMAP_URLS_COUNT = model.UrlsCount;
            }

            if (model.AttemptCount >= 3 && model.AttemptCount <= 10)
            {
                ATTEMPT_COUNT = model.AttemptCount;
            }
            
            // Init base request

            DateTime creation = DateTime.Now;
            var item = new BaseRequest
            {
                BaseRequestUrl = model.BaseRequestUrl,
                Creation = creation,
                UserId = model.UserId
            };

            context.BaseRequests.Add(item);

            await context.SaveChangesAsync();

            // Add to base request list of sitemap requests

            var userRequests = await GetBaseRequestsByUser(model.UserId);

            var baseRequest = userRequests
                .FirstOrDefault(c => c.BaseRequestUrl == model.BaseRequestUrl
                && c.Creation.Second == creation.Second);

            List<SitemapRequest> sitemapsList;

            try
            {
                sitemapsList = await GetSitemapRequests(model.BaseRequestUrl, baseRequest.Id);
            }
            catch (ValidationException exception)
            {
                await DeleteBaseRequest(baseRequest.UserId, baseRequest.BaseRequestUrl, baseRequest.Creation);
                throw exception;
            }

            baseRequest.SitemapRequests = sitemapsList;

            await context.SaveChangesAsync();

            return baseRequest;
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

            if(item == null || item == default)
            {
                return default;
            }

            return item.Id;
        }

        public async Task<BaseRequest> GetBaseRequest(int id)
         => await context.BaseRequests
            .Include(c => c.SitemapRequests)
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

                    if (time == 0)
                        break;

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
            try
            {
                await request.GetResponseAsync();
            }
            catch (Exception)
            {
                return 0;
            }
            timer.Stop();

            return Math.Round(timer.Elapsed.TotalMilliseconds);
        }

        private async Task<List<string>> GetSitemapUrls(string baseRequestUrl)
        {
            List<string> urls = new List<string> { baseRequestUrl };

            var url = new Uri(baseRequestUrl);

            var stringUrl = string.Concat(url.Scheme, "://", url.Host);

            HtmlNodeCollection nodes;
            try
            {
                var htmlDocument = await new HtmlWeb().LoadFromWebAsync(stringUrl);
                nodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            }
            catch (Exception)
            {
                throw new ValidationException("Invalid resource");
            }

            if (nodes == null || nodes.Count == 0)
            {
                throw new ValidationException("Invalid resource");
            }

            foreach (var htmlNode in nodes)
            {
                string href = htmlNode.GetAttributeValue("href", string.Empty);

                if (href.StartsWith("/") && !href.StartsWith("/#"))
                {
                    urls.Add(href);
                }
                else if (href.StartsWith(stringUrl))
                {
                    urls.Add(href);
                }

                if (urls.Distinct().Count() == SITEMAP_URLS_COUNT)
                    break;
            }
            return urls
                .Select(c => c.StartsWith("/") ? stringUrl + c : c)
                .Distinct()
                .Take(SITEMAP_URLS_COUNT)
                .ToList();
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