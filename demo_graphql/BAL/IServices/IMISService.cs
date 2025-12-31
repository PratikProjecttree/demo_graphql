using demo_graphql.Models;

namespace demo_graphql.BAL.IServices
{
    public interface IMISService
    {
        Task<IEnumerable<PositionViewModel>> GetPersonPosition(int[] personId = null);
    }
}