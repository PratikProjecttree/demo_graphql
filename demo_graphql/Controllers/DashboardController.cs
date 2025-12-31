using demo_graphql.Filters;
using Microsoft.AspNetCore.Mvc;

namespace demo_graphql.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [AccessPermissionAuthorization(new string[] { SystemModuleCode.MasterEvent }, new string[] { ModuleAction.Add })]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _dashboardService.GetDashboardSummary();
            return Ok(result);
        }
    }
}
