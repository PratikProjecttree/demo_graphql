using Dapper;
using demo_graphql.BAL.IServices;
using demo_graphql.Models;
using Npgsql;
using static demo_graphql.BAL.QueryInspector;

namespace demo_graphql.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly string _connectionString;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IConfiguration configuration, ILogger<DashboardService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string missing.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response> GetDashboardSummary()
        {
            try
            {
                _logger.LogInformation("Retrieving dashboard summary");

                var response = new Response();
                using var connection = new NpgsqlConnection(_connectionString);
                var result = await connection.QueryAsync<DashboardSummary>(PostGresQuery.Get_dashboard_summary);

                response.data = result;
                response.responseMessages.Add(new ResponseMessage { type = "S", message = "Success" });

                _logger.LogInformation("Dashboard summary retrieved successfully");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard summary");
                throw; // Re-throw to maintain original behavior
            }
        }
    }
}
