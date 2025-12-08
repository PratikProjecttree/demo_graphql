namespace demo_graphql.Controllers
{
    public class QueryInspector
    {

        public static class PostGresQuery
        {
                public const string Get_request_meta = @"SELECT
                                                         id,
                                                         object_name,
                                                         type,
                                                         category,
                                                         workflow_meta,
                                                         custom_meta,
                                                         input_validation_meta AS inputValidation,
                                                         headers,
                                                         permission_meta,
                                                         request_headers
                                                     FROM
                                                         public.request_meta
                                                     WHERE
                                                         object_name = ANY(@queryList)
                                                     ORDER BY id;";
            public const string Get_dashboard_summary = @"WITH 
                                                employee_count AS (
                                                    SELECT COUNT(*) AS total_employees FROM employee
                                                ),
                                                department_count AS (
                                                    SELECT COUNT(*) AS total_departments FROM department
                                                ),
                                                salary_stats AS (
                                                    SELECT 
                                                        MIN(salary) AS min_salary,
                                                        MAX(salary) AS max_salary,
                                                        AVG(salary) AS avg_salary,
                                                        SUM(salary) AS total_salary
                                                    FROM employee
                                                ),
                                                address_summary AS (
                                                    SELECT 
                                                        COUNT(*) AS total_addresses,
                                                        COUNT(DISTINCT employee_id) AS employees_with_address
                                                    FROM employee_address
                                                )
                                                SELECT 
                                                    (SELECT total_employees FROM employee_count) AS TotalEmployees,
                                                    (SELECT total_departments FROM department_count) AS TotalDepartments,
                                                    (SELECT min_salary FROM salary_stats) AS MinSalary,
                                                    (SELECT max_salary FROM salary_stats) AS MaxSalary,
                                                    (SELECT avg_salary FROM salary_stats) AS AvgSalary,
                                                    (SELECT total_salary FROM salary_stats) AS TotalSalary,
                                                    (SELECT employees_with_address FROM address_summary) AS EmployeesWithAddress;
                                                    ";
        }
    }
}