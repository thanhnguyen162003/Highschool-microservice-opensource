using Domain.Constants;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using StackExchange.Redis;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoolketContext _context;
        private readonly IDatabase _contextRedis;
        private ITransaction _transactionRedis = null!;

        private readonly IKetContentRepository _ketContentRepository = null!;
        private readonly IKetRepository _ketRepository = null!;
        private readonly IHistoryPlayRepository _historyPlayRepository = null!;
        private readonly ILeaderboardRepository _leaderboardRepository = null!;
        private readonly IRoomGameRepository _roomGameRepository = null!;
        private readonly IQuestionGameRepository _questionGameRepository = null!;
        private readonly IAvatarRepository _avatarRepository = null!;
        private readonly IUserRepository _userRepository = null!;

        public UnitOfWork(CoolketContext context, IConnectionMultiplexer connectionMultiplexer)
        {
            _context = context;
            _contextRedis = connectionMultiplexer.GetDatabase();
        }

        public IKetContentRepository KetContentRepository => _ketContentRepository ?? new KetContentRepository(_context);
        public IKetRepository KetRepository => _ketRepository ?? new KetRepository(_context);
        public IHistoryPlayRepository HistoryPlayRepository => _historyPlayRepository ?? new HistoryPlayRepository(_context);
        public ILeaderboardRepository LeaderboardRepository => _leaderboardRepository ?? new LeaderboardRepository(_contextRedis, StorageRedis.Leaderboard);
        public IRoomGameRepository RoomGameRepository => _roomGameRepository ?? new RoomGameRepository(_contextRedis, StorageRedis.RoomGame);
        public IQuestionGameRepository QuestionGameRepository => _questionGameRepository ?? new QuestionGameRepository(_contextRedis, StorageRedis.QuestionGame);
        public IAvatarRepository AvatarRepository => _avatarRepository ?? new AvatarRepository(_context);
        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task BeginTransaction()
        {
            _transactionRedis = _contextRedis.CreateTransaction();
            _context.Database.BeginTransaction();

            return Task.CompletedTask;
        }

        public async Task<bool> CommitTransaction()
        {
            try
            {
                await _transactionRedis.ExecuteAsync();
                await _context.Database.CommitTransactionAsync();

                return true;
            } catch
            {
                return false;
            }

        }

        public async Task<Dictionary<string, object>> HealthCheck()
        {
            var healthCheckTasks = new List<Task<(string, object)>>()
            {
                CheckSqlServerHealthAsync(),
                CheckRedisHealthAsync()
            };

            var results = await Task.WhenAll(healthCheckTasks);

            return results.ToDictionary(result => result.Item1, result => result.Item2);
        }

        private async Task<(string, object)> CheckSqlServerHealthAsync()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return ("Posgres Server", canConnect ? "Connected" : "Not Connected");
            } catch (Exception ex)
            {
                return ("Posgres Server", new
                {
                    Status = "Not Connected",
                    Error = ex.Message
                });
            }
        }

        private async Task<(string, object)> CheckRedisHealthAsync()
        {
            try
            {
                var canPing = await _contextRedis.PingAsync();
                return await Task.FromResult(("Redis", (canPing.TotalMilliseconds > 0) ? "Connected" : "Not Connected"));
            } catch (Exception ex)
            {
                return ("Redis", new
                {
                    Status = "Not Connected",
                    Error = ex.Message
                });
            }
        }

    }
}
