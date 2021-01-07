using EvaluatingWebsitePerformance.Data.Entities;
using System.Threading.Tasks;

namespace EvaluatingWebsitePerformance.BusinessLogic.Interfaces
{
    public interface IService
    {
        Task<BaseRequest> AddBaseRequest(string baseRequestUrl, string userId);

        Task<BaseRequest> GetBaseRequest(string userId, string baseRequestUrl);
    }
}
