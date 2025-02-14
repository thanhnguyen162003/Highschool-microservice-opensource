using System.ComponentModel;
using Application.Caches.CacheRepository;
using Application.Common.Interfaces.ClaimInterface;
using Application.Services.CacheService.Intefaces;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.UoW;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction _transaction;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IFlashcardRepository _flashcardRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IFlashcardContentRepository _flashcardContentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ITheoryRepository _theoryRepository;
    private readonly IOrdinaryDistributedCache _distributedCache;
    private readonly IDocumentRepository _documentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IEnrollmentProgressRepository _enrollmentProcessRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProvinceRepository _provinceRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly ICurriculumRepository _curriculumRepository;
    private readonly ISubjectCurriculumRepository _subjectCurriculumRepository;
    private readonly IUserFlashcardProgressRepository _userFlashcardProgressRepository;
    private readonly IFlashcardStudySessionRepository _flashcardStudySessionRepository;
    private readonly IUserLikeRepository _userLikeRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionAnswerRepository _questionAnswerRepository;
    private readonly IUserQuizProgressRepository _userQuizProgressRepository;
    private readonly IFlashcardFolderRepository _flashcardFolderRepository;
    private readonly IFolderUserRepository _folderUserRepository;
    private readonly IDocumentFolderRepository _documentFolderRepository;
    private readonly IContainerRepository _containerRepository;

    private readonly DocumentDbContext _context;
    private readonly IClaimInterface _claimInterface;
    private readonly ICleanCacheService _cleanCacheService;
    public UnitOfWork(DocumentDbContext context, IOrdinaryDistributedCache distributedCache,
        IClaimInterface claimInterface, ICleanCacheService cleanCacheService)
    {
        _context = context;
        _distributedCache = distributedCache;
        _claimInterface = claimInterface;
        _cleanCacheService = cleanCacheService;
    }

    public ISubjectRepository SubjectRepository =>
        _subjectRepository ?? new SubjectCacheRepository(new SubjectRepository(_context), _distributedCache,_cleanCacheService,_claimInterface, _context);
    public IFlashcardRepository FlashcardRepository => _flashcardRepository ?? new FlashcardRepository(_context);
    public IFlashcardContentRepository FlashcardContentRepository => _flashcardContentRepository ?? new FlashcardContentRepository(_context);
    public IChapterRepository ChapterRepository => _chapterRepository ?? new ChapterCacheRepository(new ChapterRepository(_context), _distributedCache, _context);
    public ILessonRepository LessonRepository => _lessonRepository ?? new LessonRepository(_context);
    public ITheoryRepository TheoryRepository => _theoryRepository ?? new TheoryRepository(_context);
    public IDocumentRepository DocumentRepository => _documentRepository ?? new DocumentRepository(_context);
    public ICategoryRepository CategoryRepository => _categoryRepository ?? new CategoryRepository(_context);
    public IEnrollmentProgressRepository EnrollmentProgressRepository => _enrollmentProcessRepository ?? new EnrollmentProgressRepository(_context);
    public IEnrollmentRepository EnrollmentRepository => _enrollmentRepository ?? new EnrollmentRepository(_context);
    public IProvinceRepository ProvinceRepository => 
        _provinceRepository ?? new ProvinceCacheRepository(new ProvinceRepository(_context), _distributedCache,_cleanCacheService, _context);
    public ISchoolRepository SchoolRepository => 
        _schoolRepository ?? new SchoolCacheRepository(new SchoolRepository(_context), _distributedCache,_cleanCacheService, _context);
    public ICurriculumRepository CurriculumRepository => _curriculumRepository ?? new CurriculumRepository(_context);
    public IFlashcardStudySessionRepository FlashcardStudySessionRepository => _flashcardStudySessionRepository ?? new FlashcardStudySessionRepository(_context);
    public IUserFlashcardProgressRepository UserFlashcardProgressRepository => _userFlashcardProgressRepository ?? new UserFlashcardProgressRepository(_context);
    public ISubjectCurriculumRepository SubjectCurriculumRepository => _subjectCurriculumRepository ?? new SubjectCurriculumRepository(_context);
    public IUserLikeRepository UserLikeRepository => _userLikeRepository ?? new UserLikeRepository(_context);
    public IQuestionRepository QuestionRepository => _questionRepository ?? new QuestionRepository(_context);
    public IQuestionAnswerRepository QuestionAnswerRepository => _questionAnswerRepository ?? new QuestionAnswerRepository(_context);
    public IUserQuizProgressRepository UserQuizProgressRepository => _userQuizProgressRepository ?? new UserQuizProgressRepository(_context);
    public IFlashcardFolderRepository FlashcardFolderRepository => _flashcardFolderRepository ?? new FlashcardFolderRepository(_context);
    public IFolderUserRepository FolderUserRepository => _folderUserRepository ?? new FolderUserRepository(_context);
    public IDocumentFolderRepository DocumentFolderRepository => _documentFolderRepository ?? new DocumentFolderRepository(_context);
    public IContainerRepository ContainerRepository => _containerRepository ?? new ContainerRepository(_context);

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        if (_context != null)
        {
            _context.Dispose();
        }
    }

    // Testing Transaction
    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            return;
        }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

}