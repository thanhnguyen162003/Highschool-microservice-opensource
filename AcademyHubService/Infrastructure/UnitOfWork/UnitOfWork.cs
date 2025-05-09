using Infrastructure.Persistence;
using Infrastructure.Repositories;
using StackExchange.Redis;

namespace Infrastructure
{
    public class UnitOfWork(AcademyHubContext context, IConnectionMultiplexer connectionMultiplexer) : IUnitOfWork
    {
        private readonly AcademyHubContext _context = context;

        private readonly IAssignmentRepository _assignmentRepository = null!;
        private readonly IGroupRepository _groupRepository = null!;
        private readonly IPendingZoneInviteRepository _pendingZoneInviteRepository = null!;
        private readonly ITestContentRepository _testContentRepository = null!;
        private readonly ISubmissionRepository _submissionRepository = null!;
        private readonly IZoneRepository _zoneRepository = null!;
        private readonly IZoneBanRepository _zoneBanRepository = null!;
        private readonly IZoneMembershipRepository _zoneMembershipRepository = null!;

        public IAssignmentRepository AssignmentRepository => _assignmentRepository ?? new AssignmentRepository(_context);
        public IGroupRepository GroupRepository => _groupRepository ?? new GroupRepository(_context);
        public IPendingZoneInviteRepository PendingZoneInviteRepository => _pendingZoneInviteRepository ?? new PendingZoneInviteRepository(_context);
        public ITestContentRepository TestContentRepository => _testContentRepository ?? new TestContentRepository(_context);
        public ISubmissionRepository SubmissionRepository => _submissionRepository ?? new SubmissionRepository(_context);
        public IZoneRepository ZoneRepository => _zoneRepository ?? new ZoneRepository(_context);
        public IZoneBanRepository ZoneBanRepository => _zoneBanRepository ?? new ZoneBanRepository(_context);
        public IZoneMembershipRepository ZoneMembershipRepository => _zoneMembershipRepository ?? new ZoneMembershipRepository(_context);

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
            _context.Database.BeginTransaction();

            return Task.CompletedTask;
        }

        public async Task<bool> CommitTransaction()
        {
            try
            {
                await _context.Database.CommitTransactionAsync();

                return true;
            } catch
            {
                return false;
            }

        }

    }
}
