namespace Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IAssignmentRepository AssignmentRepository { get; }
        public IGroupRepository GroupRepository { get; }
        public IPendingZoneInviteRepository PendingZoneInviteRepository { get; }
        public ITestContentRepository TestContentRepository { get; }
        public ISubmissionRepository SubmissionRepository { get; }
        public IZoneRepository ZoneRepository { get; }
        public IZoneBanRepository ZoneBanRepository { get; }
        public IZoneMembershipRepository ZoneMembershipRepository { get; }

        Task<bool> SaveChangesAsync();
        Task BeginTransaction();
        Task<bool> CommitTransaction();

    }
}
