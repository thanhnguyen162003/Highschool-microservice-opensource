using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
	private readonly UserDatabaseContext _context;
	private IDbContextTransaction _transaction;

	private readonly IUserRepository _userRepository;
	private readonly IStudentRepository _studentRepository;
	private readonly ITeacherRepository _teacherRepository;
	private readonly IReportRepository _reportRepository;
	private readonly IImageReportRepository _imageReportRepository;
	private readonly IUserSubjectRepository _userSubjectRepository;
	private readonly IRoadmapRepository _roadmapRepository;
	private readonly ISessionRepository _sessionRepository;
	private readonly IReportDocumentRepository _reportDocumentRepository;
	private readonly IOutboxMessage _outboxMessageRepository;
	private readonly IChosenSubjectCurriculumRepository _chosenSubjectCurriculumRepository;


	public UnitOfWork(UserDatabaseContext context)
	{
		_context = context;
		// _distributedCache = distributedCache;
	}

	public UnitOfWork()
	{
		_context = new UserDatabaseContext();
	}

	public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);
	public IStudentRepository StudentRepository => _studentRepository ?? new StudentRepository(_context);
	public ITeacherRepository TeacherRepository => _teacherRepository ?? new TeacherRepository(_context);
	public IReportRepository ReportRepository => _reportRepository ?? new ReportRepository(_context);
	public IImageReportRepository ImageReportRepository => _imageReportRepository ?? new ImageReportRepository(_context);
	public IUserSubjectRepository UserSubjectRepository => _userSubjectRepository ?? new UserSubjectRepository(_context);
	public IRoadmapRepository RoadmapRepository => _roadmapRepository ?? new RoadmapRepository(_context);
	public ISessionRepository SessionRepository => _sessionRepository ?? new SessionRepository(_context);
	public IOutboxMessage OutboxMessageRepository => _outboxMessageRepository ?? new OutboxMessageRepository(_context);
	public IChosenSubjectCurriculumRepository ChosenSubjectCurriculumRepository => _chosenSubjectCurriculumRepository ?? new ChosenSubjectCurriculumRepository(_context);
	public IReportDocumentRepository ReportDocumentRepository => _reportDocumentRepository ?? new ReportDocumentRepository(_context);

	public void SaveChanges()
	{
		_context.SaveChanges();
	}

	public async Task<bool> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public void Dispose()
	{
		if (_context != null)
		{
			_context.Dispose();
		}
	}

	public async Task BeginTransactionAsync()
	{
		if (_transaction != null)
		{
			return;
		}

		_transaction = await _context.Database.BeginTransactionAsync();
	}

	public async Task<bool> CommitTransactionAsync()
	{
		bool result = false;
		try
		{
			result = await _context.SaveChangesAsync() > 0;
			await _transaction.CommitAsync();
		} finally
		{
			_transaction.Dispose();
			_transaction = null;
		}

		return result;
	}

	public async Task RollbackTransactionAsync()
	{
		try
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync();
			}
		} finally
		{
			_transaction?.Dispose();
			_transaction = null;
		}
	}

}