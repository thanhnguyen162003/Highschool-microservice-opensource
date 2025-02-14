using Infrastructure.Repositories;

namespace Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IKetContentRepository KetContentRepository { get; }
        IKetRepository KetRepository { get; }
        IHistoryPlayRepository HistoryPlayRepository { get; }
        ILeaderboardRepository LeaderboardRepository { get; }
        IRoomGameRepository RoomGameRepository { get; }
        IQuestionGameRepository QuestionGameRepository { get; }
        IAvatarRepository AvatarRepository { get; }
        IUserRepository UserRepository { get; }

        Task<bool> SaveChangesAsync();
        Task BeginTransaction();
        Task<bool> CommitTransaction();
        Task<Dictionary<string, object>> HealthCheck();

    }
}
