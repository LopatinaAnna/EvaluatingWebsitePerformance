using EvaluatingWebsitePerformance.Data.Entities;
using EvaluatingWebsitePerformance.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvaluatingWebsitePerformance.BusinessLogic.Interfaces
{
    public interface IService : IDisposable
    {
        Task<BaseRequest> AddBaseRequest(CreateBaseRequestModel model);

        Task<int> GetBaseRequestId(string userId, string baseRequestUrl, DateTime creation);

        Task<List<BaseRequest>> GetBaseRequestsByUser(string userId);

        Task<BaseRequest> GetBaseRequest(int id);

        Task DeleteAllBaseRequest(string userId);

        Task DeleteBaseRequest(string userId, string baseRequestUrl, DateTime creation);
    }
}