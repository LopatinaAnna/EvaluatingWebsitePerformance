using System.Threading.Tasks;

namespace EvaluatingWebsitePerformance.BusinessLogic.Interfaces
{
    public interface IBaseRequestService
    {
        Task AddBaseRequest(string baseRequestUrl, string userId);
    }
}
