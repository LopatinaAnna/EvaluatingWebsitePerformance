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
using System.Xml.Linq;

namespace EvaluatingWebsitePerformance.BusinessLogic.Services
{
    public class Service : IService
    {
        private readonly ApplicationDbContext context;

        private const int URLS_LIMIT = 300;

        private const int REQUESTS_FOR_EACH_URL = 2;

        public Service(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<BaseRequest> AddBaseRequest(CreateBaseRequestModel model)
        {
            var item = new BaseRequest
            {
                BaseRequestUrl = model.BaseRequestUrl,
                Creation = DateTime.Now,
                UserId = model.UserId
            };

            var baseRequest = await CreateBaseRequest(item);

            List<SitemapRequest> sitemapsList;

            try
            {
                sitemapsList = await GetSitemapRequests(
                    baseRequest.BaseRequestUrl, 
                    baseRequest.Id);
            }
            catch (ValidationException exception)
            {
                await DeleteBaseRequest(
                    baseRequest.UserId, 
                    baseRequest.BaseRequestUrl, 
                    baseRequest.Creation);

                throw exception;
            }

            baseRequest.SitemapRequests = sitemapsList;

            await context.SaveChangesAsync();

            return baseRequest;
        }

        public async Task DeleteBaseRequest(string userId, string baseRequestUrl, DateTime creation)
        {
            var item = await context.BaseRequests
              .FirstOrDefaultAsync(c => 
              c.UserId == userId && 
              c.BaseRequestUrl == baseRequestUrl && 
              c.Creation.Second == creation.Second);

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

        public async Task<int> GetBaseRequestId(string userId, string baseRequestUrl, DateTime creation)
        {
            var item = await context.BaseRequests
               .FirstOrDefaultAsync(c => c.UserId == userId
               && c.BaseRequestUrl == baseRequestUrl
               && c.Creation.Second == creation.Second);

            if (item == null || item == default)
            {
                throw new ValidationException("Request not found");
            }

            return item.Id;
        }

        public async Task<BaseRequest> GetBaseRequest(int id)
         => await context.BaseRequests
            .Include(c => c.SitemapRequests)
            .FirstOrDefaultAsync(c => c.Id == id);


        public async Task<List<BaseRequest>> GetBaseRequestsByUser(string userId)
            => await context.BaseRequests
            .Where(c => c.UserId == userId)
            .ToListAsync();
        

        #region Helpers

        private async Task<BaseRequest> CreateBaseRequest(BaseRequest baseRequest)
        {
            context.BaseRequests.Add(baseRequest);

            await context.SaveChangesAsync();

            var userRequests = await GetBaseRequestsByUser(baseRequest.UserId);

            return userRequests
                .FirstOrDefault(c => c.BaseRequestUrl == baseRequest.BaseRequestUrl
                && c.Creation.Second == baseRequest.Creation.Second);
        }

        private async Task CreateSitemapRequest(SitemapRequest sitemapRequest)
        {
            context.SitemapRequests.Add(sitemapRequest);
            await context.SaveChangesAsync();
        }

        private async Task<List<SitemapRequest>> GetSitemapRequests(string baseRequestUrl, int baseRequestId)
        {
            List<string> urlsList;
            try
            {
                urlsList = await GetSitemapUrls(baseRequestUrl);
            }
            catch (ValidationException exeption)
            {
                throw exeption;
            }

            List<SitemapRequest> sitemapRequests = new List<SitemapRequest>();

            foreach (var sitemapUrl in urlsList)
            {
                SitemapRequest sitemapRequest;
                try
                {
                    sitemapRequest = await GetSitemapRequest(sitemapUrl, baseRequestId);
                }
                catch
                {
                    continue;
                }
                sitemapRequests.Add(sitemapRequest);
            }

            return sitemapRequests;
        }

        private async Task<List<string>> GetSitemapUrls(string baseRequestUrl)
        {
            List<string> urlsList;

            var url = new Uri(baseRequestUrl);

            var baseUrl = string.Concat(url.Scheme, "://", url.Host, "/");

            try
            {
                urlsList = GetSitemapUrlsFromXml(baseUrl);
            }
            catch
            {
                try
                {
                    urlsList = await GetSitemapUrlsFromHtml(baseUrl);
                }
                catch (ValidationException htmlException)
                {
                    throw htmlException;
                }
            }

            return urlsList;
        }

        private async Task<SitemapRequest> GetSitemapRequest(string sitemapUrl, int baseRequestId)
        {
            var sitemapRequest = new SitemapRequest
            {
                SitemapRequestUrl = sitemapUrl,
                BaseRequestId = baseRequestId
            };

            for (int i = 0; i < REQUESTS_FOR_EACH_URL; i++)
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

                await CreateSitemapRequest(sitemapRequest);
            }
            else
            {
                throw new ValidationException($"No response from {sitemapUrl}");
            }

            return sitemapRequest;
        }

        private async Task<List<string>> GetSitemapUrlsFromHtml(string baseUrl)
        {
            var urlsList = new List<string> { baseUrl };

            for (int i = 0; i < urlsList.Count && urlsList.Count <= URLS_LIMIT; i++)
            {
                HtmlNodeCollection nodes;
                try
                {
                    var htmlDocument = await new HtmlWeb().LoadFromWebAsync(urlsList[i]);
                    nodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
                }
                catch (Exception)
                {
                    continue;
                }

                if (nodes == null || nodes.Count == 0)
                {
                    continue;
                }

                foreach (var htmlNode in nodes)
                {
                    if (urlsList.Count > URLS_LIMIT)
                    {
                        break;
                    }

                    string href = htmlNode.GetAttributeValue("href", string.Empty);

                    href = (href != "/" && href.StartsWith("/") && !href.StartsWith("/#")) ? (baseUrl + href.Substring(1)) : href;

                    href = href.Split('?')[0].Split('#')[0];

                    if (href.StartsWith(baseUrl) && !urlsList.Contains(href))
                    {
                        urlsList.Add(href);
                    }
                }
            }

            return urlsList;
        }

        private List<string> GetSitemapUrlsFromXml(string baseUrl)
        {
            var urlsList = new List<string>();

            XDocument xDocument;

            XNamespace xNamespace;

            try
            {
                xDocument = XDocument.Load(baseUrl + "sitemap.xml");

                xNamespace = xDocument.Root.Name.Namespace;
            }
            catch (Exception)
            {
                throw new ValidationException("Failed to load xml");
            }

            foreach (var xElement in xDocument.Root.Elements())
            {
                var locElement = xElement.Element(xNamespace + "loc");

                if (locElement != null && !locElement.Value.EndsWith(".xml"))
                {
                    urlsList.Add(locElement.Value);
                }
            }

            if (urlsList.Count() == 0)
            {
                throw new ValidationException("Failed to load xml");
            }

            return urlsList.Distinct().ToList();
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

        #endregion Helpers
    
    }
}