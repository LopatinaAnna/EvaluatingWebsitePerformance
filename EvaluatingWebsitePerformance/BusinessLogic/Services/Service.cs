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

        public Service(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<BaseRequest> AddBaseRequest(CreateBaseRequestModel model)
        {
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

            if (item == null || item == default)
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
            List<SitemapRequest> sitemapRequests = new List<SitemapRequest>();

            List<string> foundUrls = new List<string>();

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

                href = (href.StartsWith("/") && !href.StartsWith("/#")) ? (stringUrl + href) : href;

                if (href.StartsWith(stringUrl) && !foundUrls.Contains(href))
                {
                    foundUrls.Add(href);
                    SitemapRequest sitemapRequest;
                    try
                    {
                        sitemapRequest = await GetSitemapRequest(href, baseRequestId);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    sitemapRequests.Add(sitemapRequest);
                }
            }

            return sitemapRequests;
        }

        private async Task<SitemapRequest> GetSitemapRequest(string sitemapUrl, int baseRequestId)
        {
            var sitemapRequest = new SitemapRequest 
            { 
                SitemapRequestUrl = sitemapUrl, 
                BaseRequestId = baseRequestId 
            };

            for (int i = 0; i < ATTEMPT_COUNT; i++)
            {
                double time = await GetRequestTime(sitemapUrl);

                if (time == 0)
                {
                    throw new ValidationException($"No response from {sitemapUrl}");
                }

                sitemapRequest.ResponseTimes.Add(time);
            }

            if (sitemapRequest.ResponseTimes.Count >= 2)
            {
                sitemapRequest.MinResponseTime = sitemapRequest.ResponseTimes.Min();
                sitemapRequest.MaxResponseTime = sitemapRequest.ResponseTimes.Max();

                await AddSitemapRequest(sitemapRequest);
            }
            else
            {
                throw new ValidationException($"No response from {sitemapUrl}");
            }

            return sitemapRequest;
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