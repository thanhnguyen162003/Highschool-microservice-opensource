using System.ComponentModel;
using Application.Caches.CacheRepository;
using Application.Common.Interfaces.ClaimInterface;
using Application.Services.CacheService.Intefaces;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.UoW;

public class UnitOfWork(
    DocumentDbContext context,
    IOrdinaryDistributedCache distributedCache,
    IClaimInterface claimInterface,
    ICleanCacheService cleanCacheService)
    : IUnitOfWork
{
    private IDbContextTransaction _transaction;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IFlashcardRepository _flashcardRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IFlashcardContentRepository _flashcardContentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ITheoryRepository _theoryRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IMasterSubjectRepository _masterSubjectRepository;
    private readonly IEnrollmentProgressRepository _enrollmentProcessRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProvinceRepository _provinceRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly ICurriculumRepository _curriculumRepository;
    private readonly ISubjectCurriculumRepository _subjectCurriculumRepository;
    private readonly IUserFlashcardProgressRepository _userFlashcardProgressRepository;
    private readonly IUserLikeRepository _userLikeRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionAnswerRepository _questionAnswerRepository;
    private readonly IUserQuizProgressRepository _userQuizProgressRepository;
    private readonly IFlashcardFolderRepository _flashcardFolderRepository;
    private readonly IFolderUserRepository _folderUserRepository;
    private readonly IDocumentFolderRepository _documentFolderRepository;
    private readonly IContainerRepository _containerRepository;
    private readonly IStarredTermRepository _starredTermRepository;
    private readonly IStudiableTermRepository _studiableTermRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IFlashcardTagRepository _flashcardTagRepository;
    private readonly IFSRSPresetRepository _fSRSPresetRepository;

    public ISubjectRepository SubjectRepository =>
        _subjectRepository ?? new SubjectCacheRepository(new SubjectRepository(context), distributedCache,cleanCacheService,claimInterface, context);
    public IFlashcardRepository FlashcardRepository => _flashcardRepository ?? new FlashcardRepository(context);
    public IFlashcardContentRepository FlashcardContentRepository => _flashcardContentRepository ?? new FlashcardContentRepository(context);
    public IChapterRepository ChapterRepository => _chapterRepository ?? new ChapterCacheRepository(new ChapterRepository(context), distributedCache, context);
    public ILessonRepository LessonRepository => _lessonRepository ?? new LessonRepository(context);
    public ITheoryRepository TheoryRepository => _theoryRepository ?? new TheoryRepository(context);
    public IDocumentRepository DocumentRepository => _documentRepository ?? new DocumentRepository(context);
    public IMasterSubjectRepository MasterSubjectRepository => _masterSubjectRepository ?? new MasterSubjectRepository(context);
    public IEnrollmentProgressRepository EnrollmentProgressRepository => _enrollmentProcessRepository ?? new EnrollmentProgressRepository(context);
    public IEnrollmentRepository EnrollmentRepository => _enrollmentRepository ?? new EnrollmentRepository(context);
    public IProvinceRepository ProvinceRepository => 
        _provinceRepository ?? new ProvinceCacheRepository(new ProvinceRepository(context), distributedCache,cleanCacheService, context);
    public ISchoolRepository SchoolRepository => 
        _schoolRepository ?? new SchoolCacheRepository(new SchoolRepository(context), distributedCache,cleanCacheService, context);
    public ICurriculumRepository CurriculumRepository => _curriculumRepository ?? new CurriculumRepository(context);
    public IUserFlashcardProgressRepository UserFlashcardProgressRepository => _userFlashcardProgressRepository ?? new UserFlashcardProgressRepository(context);
    public ISubjectCurriculumRepository SubjectCurriculumRepository => _subjectCurriculumRepository ?? new SubjectCurriculumRepository(context);
    public IUserLikeRepository UserLikeRepository => _userLikeRepository ?? new UserLikeRepository(context);
    public IQuestionRepository QuestionRepository => _questionRepository ?? new QuestionRepository(context);
    public IQuestionAnswerRepository QuestionAnswerRepository => _questionAnswerRepository ?? new QuestionAnswerRepository(context);
    public IUserQuizProgressRepository UserQuizProgressRepository => _userQuizProgressRepository ?? new UserQuizProgressRepository(context);
    public IFlashcardFolderRepository FlashcardFolderRepository => _flashcardFolderRepository ?? new FlashcardFolderRepository(context);
    public IFolderUserRepository FolderUserRepository => _folderUserRepository ?? new FolderUserRepository(context);
    public IDocumentFolderRepository DocumentFolderRepository => _documentFolderRepository ?? new DocumentFolderRepository(context);
    public IContainerRepository ContainerRepository => _containerRepository ?? new ContainerRepository(context);
    public IStarredTermRepository StarredTermRepository => _starredTermRepository ?? new StarredTermRepository(context);
    public IStudiableTermRepository StudiableTermRepository => _studiableTermRepository ?? new StudiableTermRepository(context);
    public ITagRepository TagRepository => _tagRepository ?? new TagRepository(context);
    public IFlashcardTagRepository FlashcardTagRepository => _flashcardTagRepository ?? new FlashcardTagRepository(context);
    public IFSRSPresetRepository FSRSPresetRepository => _fSRSPresetRepository ?? new FSRSPresetRepository(context);
    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        if (context != null)
        {
            context.Dispose();
        }
    }

    // Testing Transaction
    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            return;
        }

        _transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await context.SaveChangesAsync();
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