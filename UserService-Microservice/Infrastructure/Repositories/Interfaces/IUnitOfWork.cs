namespace Infrastructure.Repositories.Interfaces;

public interface IUnitOfWork
{
	IUserRepository UserRepository { get; }
	IStudentRepository StudentRepository { get; }
	ITeacherRepository TeacherRepository { get; }
	IReportRepository ReportRepository { get; }
	IImageReportRepository ImageReportRepository { get; }
	IUserSubjectRepository UserSubjectRepository { get; }
	IRoadmapRepository RoadmapRepository { get; }
	IReportDocumentRepository ReportDocumentRepository { get; }
	IOutboxMessage OutboxMessageRepository { get; }
	IChosenSubjectCurriculumRepository ChosenSubjectCurriculumRepository { get; }
	ISessionRepository SessionRepository { get; }


    void SaveChanges();
	Task<bool> SaveChangesAsync();
	Task BeginTransactionAsync();
	Task<bool> CommitTransactionAsync();
	Task RollbackTransactionAsync();
}