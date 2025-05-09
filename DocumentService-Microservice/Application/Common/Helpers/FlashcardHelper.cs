using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Common.Helpers;

public static class FlashcardHelper
{
    /// <summary>
    /// Tải thông tin chi tiết về các thực thể liên quan với flashcard
    /// </summary>
    /// <param name="model">Model flashcard cần bổ sung thông tin</param>
    /// <param name="flashcard">Thực thể flashcard làm nguồn dữ liệu</param>
    /// <param name="unitOfWork">Unit of work để truy cập repository</param>
    /// <returns>Task</returns>
    public static async Task LoadRelatedEntities(FlashcardModel model, Flashcard flashcard, IUnitOfWork unitOfWork)
    {
        // Đảm bảo EntityId được cập nhật từ các trường ID cũ
        model.EntityId = flashcard.FlashcardType switch
        {
            FlashcardType.Lesson => flashcard.LessonId,
            FlashcardType.Chapter => flashcard.ChapterId,
            FlashcardType.SubjectCurriculum => flashcard.SubjectCurriculumId,
            FlashcardType.Subject => flashcard.SubjectId,
            _ => null
        };
        
        if (flashcard.FlashcardType.HasValue && model.EntityId.HasValue && model.EntityId != Guid.Empty)
        {
            switch (flashcard.FlashcardType.Value)
            {
                case FlashcardType.Lesson:
                    var lesson = await unitOfWork.LessonRepository.GetByIdAsync(model.EntityId.Value, CancellationToken.None);
                    model.EntityName = lesson?.LessonName;
                    model.EntitySlug = lesson?.Slug;
                    break;
                    
                case FlashcardType.Chapter:
                    var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(model.EntityId.Value, CancellationToken.None);
                    model.EntityName = chapter?.ChapterName;
                    break;
                    
                case FlashcardType.SubjectCurriculum:
                    var curriculum = await unitOfWork.SubjectCurriculumRepository.GetByIdAsync(model.EntityId.Value, CancellationToken.None);
                    model.EntityName = curriculum?.SubjectCurriculumName;
                    break;
                    
                case FlashcardType.Subject:
                    var subject = await unitOfWork.SubjectRepository.GetByIdAsync(model.EntityId.Value, CancellationToken.None);
                    model.EntityName = subject?.SubjectName;
                    model.EntitySlug = subject?.Slug;
                    break;
            }
        }
    }
    
    /// <summary>
    /// Tải thông tin liên quan cho một danh sách các FlashcardModel
    /// </summary>
    /// <param name="models">Danh sách FlashcardModel</param>
    /// <param name="unitOfWork">UnitOfWork</param>
    /// <returns>Task</returns>
    public static async Task LoadRelatedEntitiesForList(List<FlashcardModel> models, IUnitOfWork unitOfWork)
    {
        if (models == null || !models.Any())
            return;
        
        // Tạo các dictionary để cache thông tin từ database
        var lessonDict = new Dictionary<Guid, (string Name, string Slug)>();
        var chapterDict = new Dictionary<Guid, string>();
        var curriculumDict = new Dictionary<Guid, string>();
        var subjectDict = new Dictionary<Guid, (string Name, string Slug)>();
        
        // Phân loại các model theo FlashcardType
        var lessonModels = models.Where(m => m.FlashcardType == FlashcardType.Lesson && m.EntityId.HasValue && m.EntityId != Guid.Empty).ToList();
        var chapterModels = models.Where(m => m.FlashcardType == FlashcardType.Chapter && m.EntityId.HasValue && m.EntityId != Guid.Empty).ToList();
        var curriculumModels = models.Where(m => m.FlashcardType == FlashcardType.SubjectCurriculum && m.EntityId.HasValue && m.EntityId != Guid.Empty).ToList();
        var subjectModels = models.Where(m => m.FlashcardType == FlashcardType.Subject && m.EntityId.HasValue && m.EntityId != Guid.Empty).ToList();
        
        // Lấy danh sách các ID cần truy vấn
        var lessonIds = lessonModels.Select(m => m.EntityId.Value).Distinct().ToList();
        var chapterIds = chapterModels.Select(m => m.EntityId.Value).Distinct().ToList();
        var curriculumIds = curriculumModels.Select(m => m.EntityId.Value).Distinct().ToList();
        var subjectIds = subjectModels.Select(m => m.EntityId.Value).Distinct().ToList();
        
        // Tải dữ liệu cho các bài học
        foreach (var lessonId in lessonIds)
        {
            var lesson = await unitOfWork.LessonRepository.GetByIdAsync(lessonId, CancellationToken.None);
            if (lesson != null)
            {
                lessonDict[lessonId] = (lesson.LessonName, lesson.Slug);
            }
        }
        
        // Tải dữ liệu cho các chương
        foreach (var chapterId in chapterIds)
        {
            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(chapterId, CancellationToken.None);
            if (chapter != null)
            {
                chapterDict[chapterId] = chapter.ChapterName;
            }
        }
        
        // Tải dữ liệu cho các chương trình môn học
        foreach (var curriculumId in curriculumIds)
        {
            var curriculum = await unitOfWork.SubjectCurriculumRepository.GetByIdAsync(curriculumId, CancellationToken.None);
            if (curriculum != null)
            {
                curriculumDict[curriculumId] = curriculum.SubjectCurriculumName;
            }
        }
        
        // Tải dữ liệu cho các môn học
        foreach (var subjectId in subjectIds)
        {
            var subject = await unitOfWork.SubjectRepository.GetByIdAsync(subjectId, CancellationToken.None);
            if (subject != null)
            {
                subjectDict[subjectId] = (subject.SubjectName, subject.Slug);
            }
        }
        
        // Gán dữ liệu cho các model
        foreach (var model in lessonModels)
        {
            if (model.EntityId.HasValue && lessonDict.TryGetValue(model.EntityId.Value, out var lessonInfo))
            {
                model.EntityName = lessonInfo.Name;
                model.EntitySlug = lessonInfo.Slug;
            }
        }
        
        foreach (var model in chapterModels)
        {
            if (model.EntityId.HasValue && chapterDict.TryGetValue(model.EntityId.Value, out var chapterName))
            {
                model.EntityName = chapterName;
            }
        }
        
        foreach (var model in curriculumModels)
        {
            if (model.EntityId.HasValue && curriculumDict.TryGetValue(model.EntityId.Value, out var curriculumName))
            {
                model.EntityName = curriculumName;
            }
        }
        
        foreach (var model in subjectModels)
        {
            if (model.EntityId.HasValue && subjectDict.TryGetValue(model.EntityId.Value, out var subjectInfo))
            {
                model.EntityName = subjectInfo.Name;
                model.EntitySlug = subjectInfo.Slug;
            }
        }
    }
    
    /// <summary>
    /// Lọc danh sách flashcard dựa trên EntityId và FlashcardType
    /// </summary>
    /// <param name="flashcards">Danh sách flashcard cần lọc</param>
    /// <param name="entityId">ID của entity</param>
    /// <param name="flashcardType">Loại flashcard</param>
    /// <returns>Danh sách flashcard đã lọc</returns>
    public static IEnumerable<Flashcard> FilterByEntity(
        IEnumerable<Flashcard> flashcards,
        Guid entityId,
        FlashcardType flashcardType)
    {
        // Kiểm tra danh sách đầu vào
        if (flashcards == null || !flashcards.Any())
            return Enumerable.Empty<Flashcard>();
            
        // Kiểm tra nếu EntityId không hợp lệ
        if (entityId == Guid.Empty)
            return Enumerable.Empty<Flashcard>();
        
        // Lọc theo EntityId dựa vào loại flashcard
        switch (flashcardType)
        {
            case FlashcardType.Lesson:
                flashcards = flashcards.Where(f => f.LessonId == entityId);
                break;
            case FlashcardType.Chapter:
                flashcards = flashcards.Where(f => f.ChapterId == entityId
                                            || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.ChapterId == entityId));
                break;
            case FlashcardType.SubjectCurriculum:
                flashcards = flashcards.Where(f => f.SubjectCurriculumId == entityId
                                            || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.Chapter.SubjectCurriculumId == entityId)
                                            || (f.ChapterId != null && f.ChapterId != Guid.Empty && f.Chapter!.SubjectCurriculumId == entityId));
                break;
            case FlashcardType.Subject:
                flashcards = flashcards.Where(f => f.SubjectId == entityId
                                            || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.Chapter.SubjectCurriculum.SubjectId == entityId)
                                            || (f.ChapterId != null && f.ChapterId != Guid.Empty && f.Chapter!.SubjectCurriculum.SubjectId == entityId)
                                            || (f.SubjectCurriculumId != null && f.SubjectCurriculumId != Guid.Empty && f.SubjectCurriculum!.SubjectId == entityId));
                break;
        }
        
        return flashcards;
    }
    
    /// <summary>
    /// Kiểm tra tính hợp lệ của các mối quan hệ trong flashcard
    /// </summary>
    /// <param name="flashcards">Danh sách flashcard cần lọc</param>
    /// <param name="entityId">ID của entity (nếu có)</param>
    /// <param name="flashcardType">Loại flashcard (nếu có)</param>
    /// <returns>Danh sách flashcard đã lọc</returns>
    public static IEnumerable<Flashcard> FilterByRelationships(
        IEnumerable<Flashcard> flashcards, 
        Guid? entityId = null,
        FlashcardType? flashcardType = null)
    {
        if (flashcardType.HasValue)
        {
            if (entityId.HasValue && entityId != Guid.Empty)
            {
                switch (flashcardType)
                {
                    case FlashcardType.Lesson:
                        flashcards = flashcards.Where(f => f.LessonId == entityId);
                        break;
                    case FlashcardType.Chapter:
                        flashcards = flashcards.Where(f => f.ChapterId == entityId
                                                    || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.ChapterId == entityId));
                        break;
                    case FlashcardType.SubjectCurriculum:
                        flashcards = flashcards.Where(f => f.SubjectCurriculumId == entityId
                                                    || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.Chapter.SubjectCurriculumId == entityId)
                                                    || (f.ChapterId != null && f.ChapterId != Guid.Empty && f.Chapter!.SubjectCurriculumId == entityId));
                        break;
                    case FlashcardType.Subject:
                        flashcards = flashcards.Where(f => f.SubjectId == entityId
                                                    || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.Chapter.SubjectCurriculum.SubjectId == entityId)
                                                    || (f.ChapterId != null && f.ChapterId != Guid.Empty && f.Chapter!.SubjectCurriculum.SubjectId == entityId)
                                                    || (f.SubjectCurriculumId != null && f.SubjectCurriculumId != Guid.Empty && f.SubjectCurriculum!.SubjectId == entityId));
                        break;
                }
            }           
        }
        
        return flashcards;
    }
    
    /// <summary>
    /// Kiểm tra tính hợp lệ của mối quan hệ giữa EntityId và FlashcardType
    /// </summary>
    /// <param name="entityId">ID của entity</param>
    /// <param name="flashcardType">Loại flashcard</param>
    /// <returns>True nếu hợp lệ, False nếu không hợp lệ</returns>
    public static bool ValidateEntityRelationship(
        Guid? entityId,
        FlashcardType flashcardType)
    {
        // Kiểm tra xem entityId có hợp lệ hay không
        if (!entityId.HasValue || entityId == Guid.Empty)
            return false;
            
        // Nếu entityId có giá trị hợp lệ và flashcardType có giá trị, mối quan hệ được coi là hợp lệ
        return true;
    }
    
    /// <summary>
    /// Kiểm tra sự tồn tại của entity dựa trên loại flashcard
    /// </summary>
    /// <param name="unitOfWork">Unit of work để truy cập repository</param>
    /// <param name="entityId">ID của entity</param>
    /// <param name="flashcardType">Loại flashcard</param>
    /// <returns>Tuple (isValid, errorMessage)</returns>
    public static async Task<(bool isValid, string errorMessage)> ValidateEntityExists(
        IUnitOfWork unitOfWork,
        Guid? entityId,
        FlashcardType flashcardType)
    {
        if (!entityId.HasValue || entityId == Guid.Empty)
            return (false, "ID entity không được trống");
            
        switch (flashcardType)
        {
            case FlashcardType.Lesson:
                var lesson = await unitOfWork.LessonRepository.GetByIdAsync(entityId.Value, CancellationToken.None);
                return lesson != null 
                    ? (true, string.Empty) 
                    : (false, "Bài học không tồn tại");
                
            case FlashcardType.Chapter:
                var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(entityId.Value, CancellationToken.None);
                return chapter != null 
                    ? (true, string.Empty) 
                    : (false, "Chương không tồn tại");
                
            case FlashcardType.SubjectCurriculum:
                var curriculum = await unitOfWork.SubjectCurriculumRepository.GetByIdAsync(entityId.Value, CancellationToken.None);
                return curriculum != null 
                    ? (true, string.Empty) 
                    : (false, "Chương trình môn học không tồn tại");
                
            case FlashcardType.Subject:
                var subject = await unitOfWork.SubjectRepository.GetByIdAsync(entityId.Value, CancellationToken.None);
                return subject != null 
                    ? (true, string.Empty) 
                    : (false, "Môn học không tồn tại");
                
            default:
                return (false, "Loại flashcard không hợp lệ");
        }
    }
} 