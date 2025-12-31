using demo_graphql.Models;
using demo_graphql.Models.EmailModels;

namespace demo_graphql.Controllers
{
    public interface IEmailQueueService
    {
        Task<IListResponse<EmailQueueResponse>> AddOrUpdateEmailQueue(EmailQueueRequest model);
    }
}