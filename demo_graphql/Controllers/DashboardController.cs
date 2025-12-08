using Microsoft.AspNetCore.Mvc;

namespace demo_graphql.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public DashboardController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _employeeService.GetDashboardSummary();
            return Ok(result);
        }
    }
}
