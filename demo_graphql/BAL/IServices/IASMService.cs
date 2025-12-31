

using demo_graphql.Models;

namespace demo_graphql.BAL.IServices
{
    public interface IASMService
    {
        Task<List<RoleAccessViewModel>> GetAllAccessByRolePositionId(List<RolePositionModel> model);
    }
}