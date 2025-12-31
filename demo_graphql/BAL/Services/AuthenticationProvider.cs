

using demo_graphql.BAL.IServices;
using demo_graphql.Controllers;
using demo_graphql.Models;

namespace demo_graphql.BAL.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMISService _MISProvider;
    private readonly IASMService _asmProvider;
    public AuthenticationService(IASMService asmProvider, IMISService mISProvider)
    {
        _asmProvider = asmProvider;
        _MISProvider = mISProvider;
    }

    public async Task<List<UserModuleAccessResponse>> PermissionByPosition(int positionId, int loginUserId)
    {
        var response = new List<UserModuleAccessResponse>();

        var accessPermissions = new List<UserModuleAccessResponse>();
        List<UserModuleAccessResponse> userModuleAccess = new List<UserModuleAccessResponse>();

        var userPositions = await _MISProvider.GetPersonPosition(new int[] { loginUserId });

        if (userPositions?.FirstOrDefault(f => f.PositionId == positionId) != null)
        {
            var rolePositionModel = userPositions.Where(d => d.PositionId == positionId).Select(t => new RolePositionModel()
            {
                RoleId = t.RoleId ?? 0,
                PositionId = t.PositionId
            }).ToList();


            var permissions = await _asmProvider.GetAllAccessByRolePositionId(rolePositionModel);

            foreach (var permission in permissions)
            {
                if (permission?.HasViewAccess == true)
                {
                    UserModuleAccessResponse moduleAccess = new()
                    {
                        ModuleCode = permission?.ModuleCode ?? "",
                        ModuleId = permission?.ModuleId ?? 0,
                        ModuleName = permission?.ModuleName ?? "",
                        ActionId = 1,
                        ActionName = ModuleAction.View,
                        ActionCode = ModuleAction.View,
                    };
                    userModuleAccess.Add(moduleAccess);
                }
                if (permission?.HasCreateAccess == true)
                {
                    UserModuleAccessResponse moduleAccess = new()
                    {
                        ModuleCode = permission?.ModuleCode ?? "",
                        ModuleId = permission?.ModuleId ?? 0,
                        ModuleName = permission?.ModuleName ?? "",
                        ActionId = 1,
                        ActionName = ModuleAction.Add,
                        ActionCode = ModuleAction.Add,
                    };
                    userModuleAccess.Add(moduleAccess);
                }
                if (permission?.HasUpdateAccess == true)
                {
                    UserModuleAccessResponse moduleAccess = new()
                    {
                        ModuleCode = permission?.ModuleCode ?? "",
                        ModuleId = permission?.ModuleId ?? 0,
                        ModuleName = permission?.ModuleName ?? "",
                        ActionId = 1,
                        ActionName = ModuleAction.Edit,
                        ActionCode = ModuleAction.Edit,
                    };
                    userModuleAccess.Add(moduleAccess);
                }
                if (permission?.HasDeleteAccess == true)
                {
                    UserModuleAccessResponse moduleAccess = new()
                    {
                        ModuleCode = permission?.ModuleCode ?? "",
                        ModuleId = permission?.ModuleId ?? 0,
                        ModuleName = permission?.ModuleName ?? "",
                        ActionId = 1,
                        ActionName = ModuleAction.Delete,
                        ActionCode = ModuleAction.Delete,
                    };
                    userModuleAccess.Add(moduleAccess);
                }
            }
            response = userModuleAccess;
        }

        return response;
    }
    public async Task<bool> HasModulePermission(int positionId, int userId, IEnumerable<string> moduleCodes, string objectName, string operationType)
    {
        IEnumerable<string> actionCodes = ActionCodes(objectName, operationType);
        var moduleAccessResponse = await PermissionByPosition(positionId, userId);

        var moduleAccessList = moduleAccessResponse ?? Enumerable.Empty<UserModuleAccessResponse>();

        if (!moduleAccessList.Any())
            return false;

        return moduleAccessList.Any(x =>
            moduleCodes.Contains(x.ModuleCode) &&
            actionCodes.Contains(x.ActionCode));
    }
    private static HashSet<string> ActionCodes(string objectName, string operationType)
    {
        var actions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(operationType) &&
            operationType.Contains(OperationType.Query, StringComparison.OrdinalIgnoreCase))
        {
            actions.Add(ModuleAction.View);
        }

        if (!string.IsNullOrWhiteSpace(objectName))
        {
            if (objectName.Contains(OperationType.Insert, StringComparison.OrdinalIgnoreCase))
                actions.Add(ModuleAction.Add);

            if (objectName.Contains(OperationType.Update, StringComparison.OrdinalIgnoreCase))
                actions.Add(ModuleAction.Edit);

            if (objectName.Contains(OperationType.Update, StringComparison.OrdinalIgnoreCase))
                actions.Add(ModuleAction.Delete);
        }

        return actions;
    }
}