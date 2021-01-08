using EvaluatingWebsitePerformance.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvaluatingWebsitePerformance.BusinessLogic.Interfaces
{
    public interface IService
    {
        Task<BaseRequest> AddBaseRequest(string baseRequestUrl, string userId);

        Task<int> GetBaseRequestId(string userId, string baseRequestUrl, DateTime creation);

        Task<List<BaseRequest>> GetBaseRequestsByUser(string userId);

        Task<BaseRequest> GetBaseRequests(int id);

        Task DeleteAllBaseRequest(string userId);

        Task DeleteBaseRequest(string userId, string baseRequestUrl, DateTime creation);
    }
}
