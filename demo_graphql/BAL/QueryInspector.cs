namespace demo_graphql.Controllers
{
    public class QueryInspector
    {

        public static class PostGresQuery
        {
            public const string fn_process_request = @"select * from mds.fn_process_request(@LoginPersonId, @queryList);";
        }
    }
}