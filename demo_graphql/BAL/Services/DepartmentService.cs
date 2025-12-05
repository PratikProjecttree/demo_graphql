using Dapper;
using demo_graphql.Models;
using Npgsql;
using static demo_graphql.Controllers.QueryInspector;

namespace demo_graphql.Services
{
    public class DepartmentService : Controllers.IDepartmentService
    {
        private readonly string _connectionString;

        public DepartmentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string missing.");
        }

        public async Task<Response> GetAllAsync()
        {
            Response _response = new();
            using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.QueryAsync<Department>(PostGresQuery.Get_department);

            _response.data = result;
            _response.responseMessages.Add(new ResponseMessage { type = "S", message = "Success" });
            return _response;
        }
    }
}
