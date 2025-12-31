using demo_graphql.Models;

namespace demo_graphql.BAL.IServices
{

    public interface IAuthenticationService
    {
        Task<List<UserModuleAccessResponse>> PermissionByPosition(int positionId, int loginUserId);
        Task<bool> HasModulePermission(int positionId, int userId, IEnumerable<string> moduleCodes, string objectName, string actionCodes);

    }
}