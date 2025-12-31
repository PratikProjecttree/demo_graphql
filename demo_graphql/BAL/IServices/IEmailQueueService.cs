using demo_graphql.Models;
using demo_graphql.Models.EmailModels;

namespace demo_graphql.BAL.IServices
{
    public interface IEmailQueueService
    {
        Task<IListResponse<EmailQueueResponse>> AddOrUpdateEmailQueue(EmailQueueRequest model);
    }
}