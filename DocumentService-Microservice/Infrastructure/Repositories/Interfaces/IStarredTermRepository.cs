namespace Infrastructure.Repositories.Interfaces
{
    public interface IStarredTermRepository : IRepository<StarredTerm>
    {
        Task<bool> AddStarredTerm(StarredTerm starredTerm);
        Task<bool> DeleteStarredTerm(StarredTerm starredTerm);
        Task<bool> CheckDuplicateStarredTerm(Guid userId, Guid flashcardContentId, Guid containerId);
        Task<List<StarredTerm>> GetStarredTerm(Guid userId, Guid contentId);
    }
}
