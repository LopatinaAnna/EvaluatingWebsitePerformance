using EvaluatingWebsitePerformance.BusinessLogic.Interfaces;
using EvaluatingWebsitePerformance.Data;
using EvaluatingWebsitePerformance.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EvaluatingWebsitePerformance.BusinessLogic.Services
{
    public class BaseRequestService : IBaseRequestService
    {
        private readonly ApplicationDbContext context;

        private readonly DbSet<BaseRequest> dbSet;

        public BaseRequestService(ApplicationDbContext _context)
        {
            context = _context;
            dbSet = _context.Set<BaseRequest>();
        }

        public async Task AddBaseRequest(string baseRequestUrl, string userId)
        {

            var item = new BaseRequest
            {
                BaseRequestUrl = baseRequestUrl,
                Creation = DateTime.Now,
                UserId = userId
            };

            var sitemapsList = GetSitemapRequests(baseRequestUrl);

            item.SitemapRequests = sitemapsList;

            dbSet.Add(item);

            await context.SaveChangesAsync();

        }

        private List<SitemapRequest> GetSitemapRequests(string baseRequestUrl)
        {
            GetSitemapUrls(baseRequestUrl);
            throw new NotImplementedException();
        }

        private List<string> GetSitemapUrls(string baseRequestUrl)
        {
            throw new NotImplementedException();
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