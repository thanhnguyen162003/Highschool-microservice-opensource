using Domain.Enums;
// using Infrastructure.Data.Interceptors;
using System.Diagnostics;

namespace Infrastructure.Contexts;

public partial class DocumentDbContext : DbContext
{
    public DocumentDbContext(){}

    public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) {}

    public virtual DbSet<Chapter> Chapters { get; set; }
    
    public virtual DbSet<Container> Containers { get; set; }

    public virtual DbSet<StudiableTerm> StudiableTerms { get; set; }
    
    public virtual DbSet<StarredTerm> StarredTerms { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }
    
    public virtual DbSet<Curriculum> Curricula { get; set; }
    
    public virtual DbSet<SubjectCurriculum> SubjectCurricula { get; set; }

    public virtual DbSet<ExamAnswer> ExamAnswers { get; set; }

    public virtual DbSet<Flashcard> Flashcards { get; set; }

    public virtual DbSet<FlashcardContent> FlashcardContents { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }
    
    public virtual DbSet<Province> Provinces { get; set; }
    
    public virtual DbSet<School> Schools { get; set; }
    
    public virtual DbSet<UserLike> UserLikes { get; set; }
    
    public virtual DbSet<RecommendedData> RecommendedDatas { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Theory> Theories { get; set; }

    public virtual DbSet<UserFlashcardProgress> UserFlashcardProgresses { get; set; }
    
    // public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<MasterSubject> MasterSubjects { get; set; }
    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<FolderUser> FolderUsers { get; set; }
    public virtual DbSet<FlashcardFolder> FlashcardFolders { get; set; }
    public virtual DbSet<DocumentFolder> DocumentFolders { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<FlashcardTag> FlashcardTags { get; set; }
    public virtual DbSet<FSRSPreset> FSRSPresets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FolderUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("FolderUser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.Name).HasColumnName("name");

            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.Property(e => e.Visibility).HasColumnName("visibility");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");


        });

        modelBuilder.Entity<FlashcardFolder>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("FlashcardFolder");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.FlashcardId).HasColumnName("flashcardId");

            entity.Property(e => e.FolderId).HasColumnName("folderId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");

            entity.HasOne(d => d.Folder)
                .WithMany(p => p.FlashcardFolders)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Flashcard)
                .WithMany()
                .HasForeignKey(d => d.FlashcardId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        });

        modelBuilder.Entity<DocumentFolder>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("DocumentFolder");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.DocumentId).HasColumnName("documentId");

            entity.Property(e => e.FolderId).HasColumnName("folderId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");

            entity.HasOne(d => d.Folder)
                .WithMany(p => p.DocumentFolders)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Document)
                .WithMany()
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Chapter_pkey");

            entity.ToTable("Chapter");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChapterLevel)
                .HasMaxLength(50)
                .HasColumnName("chapterLevel");
            entity.Property(e => e.ChapterName)
                .HasMaxLength(500)
                .HasColumnName("chapterName");
            entity.Property(e => e.Semester)
                .HasMaxLength(1000)
                .HasColumnName("semester");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Like).HasColumnName("like");
            entity.Property(e => e.SubjectCurriculumId).HasColumnName("subjectCurriculumId");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.SubjectCurriculum).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.SubjectCurriculumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Chapter_subjectCurriculumId_fkey");
        });
        
        modelBuilder.Entity<Curriculum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Curriculum_pkey");

            entity.ToTable("Curriculum");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CurriculumName)
                .HasMaxLength(200)
                .HasColumnName("curriculumName");
			entity.Property(e => e.ImageUrl)
				.HasMaxLength(1000)
				.HasColumnName("imageUrl");
			entity.Property(e => e.IsExternal)
				.HasColumnName("isExternal");
			entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            
        });

        modelBuilder.Entity<SubjectCurriculum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SubjectCurriculum_pkey");
            entity.ToTable("SubjectCurriculum");
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.SubjectCurriculumName)
                .HasMaxLength(1000)
                .HasColumnName("subjectCurriculumName");
            entity.Property(e => e.IsPublish)
                .HasColumnName("isPublish");
            entity.Property(e => e.SubjectId)
                .IsRequired()
                .HasColumnName("subjectId");
            entity.Property(e => e.CurriculumId)
                .IsRequired()
                .HasColumnName("curriculumId");
            entity.HasOne(sc => sc.Subject)
                .WithMany(s => s.SubjectCurricula)
                .HasForeignKey(sc => sc.SubjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SubjectCurriculum_Subject");
            entity.HasOne(sc => sc.Curriculum)
                .WithMany(c => c.SubjectCurricula)
                .HasForeignKey(sc => sc.CurriculumId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SubjectCurriculum_Curriculum");
            entity.HasMany(e => e.Enrollments)
                .WithOne(d => d.SubjectCurriculum)
                .HasForeignKey(d => d.SubjectCurriculumId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Enrollment_subjectCurriculumId_fkey");
        });
        modelBuilder.Entity<UserLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserLike_pkey");
            entity.ToTable("UserLike");
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("userId");
            entity.Property(e => e.SubjectId)
                .HasColumnName("subjectId");
            entity.Property(e => e.DocumentId)
                .HasColumnName("documentId");
            entity.Property(e => e.LessonId)
                .HasColumnName("lessonId");
            entity.Property(e => e.FlashcardId)
                .HasColumnName("flashcardId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.HasOne(sc => sc.Subject)
                .WithMany(s => s.UserLikes)
                .HasForeignKey(sc => sc.SubjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UserLike_Subject");
            entity.HasOne(sc => sc.Document)
                .WithMany(s => s.UserLikes)
                .HasForeignKey(sc => sc.DocumentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UserLike_Document");
            entity.HasOne(sc => sc.Flashcard)
                .WithMany(s => s.UserLikes)
                .HasForeignKey(sc => sc.FlashcardId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UserLike_Flashcard");
            entity.HasOne(sc => sc.Lesson)
                .WithMany(s => s.UserLikes)
                .HasForeignKey(sc => sc.LessonId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UserLike_Lesson");
        });


        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Document_pkey");

            entity.ToTable("Document");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.Download).HasColumnName("download");
            entity.Property(e => e.DocumentDescription)
                .HasMaxLength(1000)
                .HasColumnName("documentDescription");
            entity.Property(e => e.DocumentName)
                .HasMaxLength(200)
                .HasColumnName("documentName");
            entity.Property(e => e.DocumentYear)
                .HasMaxLength(4)
                .HasColumnName("documentYear");
            entity.Property(e => e.Like).HasColumnName("like");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.SubjectCurriculumId)
                .HasColumnName("subjectCurriculumId");
            entity.Property(e => e.SchoolId).HasColumnName("schoolId");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.View).HasColumnName("view");
            entity.Property(e => e.Semester).HasColumnName("semester");

            entity.HasOne(d => d.School)
                .WithMany(p => p.Documents)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Document_schoolId_fkey");
            entity.HasOne(d => d.SubjectCurriculum)
                .WithMany(d => d.Documents)
                .HasForeignKey(d => d.SubjectCurriculumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Document_subjectCurriculumId_fkey");
        });


        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Exam_pkey");

            entity.ToTable("Exam");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .HasColumnName("author");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.Download).HasColumnName("download");
            entity.Property(e => e.ExamCode)
                .HasMaxLength(50)
                .HasColumnName("examCode");
            entity.Property(e => e.ExamDescription)
                .HasMaxLength(255)
                .HasColumnName("examDescription");
            entity.Property(e => e.ExamName)
                .HasMaxLength(255)
                .HasColumnName("examName");
            entity.Property(e => e.ExamYear)
                .HasMaxLength(4)
                .HasColumnName("examYear");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.Like).HasColumnName("like");
            entity.Property(e => e.Page).HasColumnName("page");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.SubjectId).HasColumnName("subjectId");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.View).HasColumnName("view");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Exam_subjectId_fkey");
        });

        modelBuilder.Entity<ExamAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExamAnswer_pkey");

            entity.ToTable("ExamAnswer");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("createdBy");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.ExamId).HasColumnName("examId");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updatedBy");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamAnswers)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExamAnswer_examId_fkey");
        });

        modelBuilder.Entity<Flashcard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Flashcard_pkey");

            entity.ToTable("Flashcard");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("createdBy");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.FlashcardDescription)
                .HasMaxLength(1000)
                .HasColumnName("flashcardDescription");
            entity.Property(e => e.Created)
                .HasColumnName("created");
            entity.Property(e => e.IsArtificalIntelligence)
                .HasColumnName("isArtificalIntelligence");
            entity.Property(e => e.IsCreatedBySystem)
                .HasColumnName("isCreatedBySystem");
            entity.Property(e => e.FlashcardName)
                .HasMaxLength(200)
                .HasColumnName("flashcardName");
            entity.Property(e => e.Vote).HasColumnName("vote");
            entity.Property(e => e.TotalView).HasColumnName("totalView");
            entity.Property(e => e.TodayView).HasColumnName("todayView");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.Star).HasColumnName("star");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subjectId");
            entity.Property(e => e.LessonId).HasColumnName("lessonId");
            entity.Property(e => e.ChapterId).HasColumnName("chapterId");
            entity.Property(e => e.SubjectCurriculumId).HasColumnName("subjectCurriculumId");
            entity.Property(e => e.FlashcardType)
                .HasMaxLength(50)
                .HasColumnName("flashcardType")
                .HasConversion(
                    v => v.ToString(),
                    v => (FlashcardType)Enum.Parse(typeof(FlashcardType), v));
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updatedBy");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.HasOne(d => d.Subject).WithMany(p => p.Flashcards)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            entity.HasOne(d => d.Lesson)
                .WithMany(p => p.Flashcards)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("fk_flashcard_lesson")
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Chapter)
                .WithMany(p => p.Flashcards)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("fk_flashcard_chapter")
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.SubjectCurriculum)
                .WithMany(p => p.Flashcards)
                .HasForeignKey(d => d.SubjectCurriculumId)
                .HasConstraintName("fk_flashcard_subject_curriculum")
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(d => d.FlashcardTags)
                .WithOne(p => p.Flashcard)
                .HasForeignKey(d => d.FlashcardId);
        });
        
        modelBuilder.Entity<RecommendedData>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RecommendedData_pkey");

            entity.ToTable("RecommendedData");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.SubjectIds)
                .HasColumnName("subjectIds");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.TypeExam)
                .HasColumnName("typeExam");
            entity.Property(e => e.ObjectId)
                .HasColumnName("objectId");
            entity.Property(e => e.Grade).HasColumnName("grade");
        });

        modelBuilder.Entity<FlashcardContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FlashcardContent_pkey");

            entity.ToTable("FlashcardContent");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("createdBy");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.FlashcardContentDefinition)
                .HasMaxLength(1000)
                .HasColumnName("flashcardContentDefinition");
            entity.Property(e => e.FlashcardContentDefinitionRichText)
                .HasColumnName("flashcardContentDefinitionRichText");
            entity.Property(e => e.FlashcardContentTermRichText)
                .HasColumnName("flashcardContentTermRichText");
            entity.Property(e => e.Rank)
                .IsRequired()
				.HasColumnName("rank");
            entity.Property(e => e.FlashcardContentTerm)
                .HasMaxLength(1000)
                .HasColumnName("flashcardContentTerm");
            entity.Property(e => e.FlashcardId).HasColumnName("flashcardId");
            entity.Property(e => e.Image)
                .HasMaxLength(1000)
                .HasColumnName("image");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updatedBy");

            entity.HasOne(d => d.Flashcard).WithMany(p => p.FlashcardContents)
                .HasForeignKey(d => d.FlashcardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FlashcardContent_flashcardId_fkey");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Lesson_pkey");

            entity.ToTable("Lesson");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChapterId).HasColumnName("chapterId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.DisplayOrder).HasColumnName("displayOrder");
            entity.Property(e => e.LessonBody)
                .HasColumnType("character varying")
                .HasColumnName("lessonBody");
            entity.Property(e => e.LessonMaterial)
                .HasColumnType("character varying")
                .HasColumnName("lessonMaterial");
            entity.Property(e => e.LessonName)
                .HasMaxLength(200)
                .HasColumnName("lessonName");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(1000)
                .HasColumnName("videoUrl");
            entity.Property(e => e.YoutubeVideoUrl)
                .HasMaxLength(3600)
                .HasColumnName("youtubeVideoUrl");
            entity.Property(e => e.Like).HasColumnName("like");
            entity.Property(e => e.Slug)
                .HasMaxLength(1000)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lesson_chapterId_fkey");
        });
        
        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Province_pkey");

            entity.ToTable("Province");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(1000)
                .HasColumnName("provinceName");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

		});
        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("School_pkey");

            entity.ToTable("School");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.SchoolName)
                .HasMaxLength(1000)
                .HasColumnName("schoolName");
            entity.Property(e => e.LocationDetail)
                .HasMaxLength(1000)
                .HasColumnName("locationDetail");
            entity.Property(e => e.ProvinceId).HasColumnName("provinceId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            
            entity.HasOne(d => d.Province).WithMany(p => p.Schools)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("School_provinceId_fkey");
        });        

        modelBuilder.Entity<UserFlashcardProgress>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("UserFlashcardProgress");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.Property(e => e.FlashcardContentId).HasColumnName("flashcardContentId");

            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.Property(e => e.Difficulty).HasColumnName("difficulty");

            entity.Property(e => e.Stability).HasColumnName("stability");

            entity.Property(e => e.State).HasColumnName("state");

            entity.Property(e => e.DueDate)
            .HasColumnName("dueDate");

            entity.Property(e => e.LastReviewDate).HasColumnName("lastReviewDate");

            entity.Property(e => e.TimeSpent).HasColumnName("timeSpent");
            
            // Thêm cấu hình cho LastReviewDateHistory và TimeSpentHistory
            entity.Property(e => e.LastReviewDateHistory)
                .HasColumnName("lastReviewDateHistory")
                .HasColumnType("timestamp with time zone[]");

            entity.Property(e => e.TimeSpentHistory)
                .HasColumnName("timeSpentHistory")
                .HasColumnType("double precision[]");

            // Cấu hình relationship với FlashcardContent
            entity.HasOne(e => e.FlashcardContent)
                .WithMany()
                .HasForeignKey(e => e.FlashcardContentId)
                .OnDelete(DeleteBehavior.Cascade);

            //// Add index on DueDate
            //entity.HasIndex(e => e.DueDate)
            //    .HasDatabaseName("IX_UserFlashcardProgress_DueDate");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Subject_pkey");

            entity.ToTable("Subject");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Information)
                .HasMaxLength(1000)
                .HasColumnName("information");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasColumnName("category");
            entity.Property(e => e.Like).HasColumnName("like");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.SubjectDescription)
                .HasMaxLength(1000)
                .HasColumnName("subjectDescription");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(200)
                .HasColumnName("subjectName");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(100)
                .HasColumnName("subjectCode");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.View).HasColumnName("view");
            entity.Property(e => e.MasterSubjectId)
                .HasColumnName("masterSubjectId");
            entity.HasOne(s => s.MasterSubject)
                .WithMany(c => c.Subjects)
                .HasForeignKey(s => s.MasterSubjectId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(c => c.SubjectCurricula)
                .WithOne(s => s.Subject)
                .OnDelete(DeleteBehavior.Cascade);
        });
        // modelBuilder.Entity<Category>(entity =>
        // {
        //     entity.HasKey(c => c.Id).HasName("Category_pkey");
        //     entity.ToTable("Category");
        //     entity.Property(e => e.Id)
        //         .ValueGeneratedNever()
        //         .HasColumnName("id");
        //     entity.Property(c => c.CategoryName)
        //         .HasColumnName("categoryName")
        //         .HasMaxLength(255);
        //     entity.Property(c => c.CategorySlug)
        //         .HasColumnName("categorySlug")
        //         .HasMaxLength(255);
        //     entity.HasMany(c => c.Subjects)
        //         .WithOne(s => s.Category)
        //         .OnDelete(DeleteBehavior.SetNull);
        //     entity.HasIndex(c => c.CategorySlug)
        //         .HasDatabaseName("Index_Category_CategorySlug");
        // });
        
        modelBuilder.Entity<MasterSubject>(entity =>
        {
            entity.HasKey(c => c.Id).HasName("MasterSubject_pkey");
            entity.ToTable("MasterSubject");
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(c => c.MasterSubjectName)
                .HasColumnName("masterSubjectName")
                .HasMaxLength(500);
            entity.Property(c => c.MasterSubjectSlug)
                .HasColumnName("masterSubjectSlug")
                .HasMaxLength(500);
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.HasMany(c => c.Subjects)
                .WithOne(s => s.MasterSubject)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(c => c.MasterSubjectSlug)
                .HasDatabaseName("Index_MasterSubject_MasterSubjectSlug");
        });

        modelBuilder.Entity<Theory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Theory_pkey");

            entity.ToTable("Theory");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.LessonId).HasColumnName("lessonId");
            entity.Property(e => e.TheoryContentHtml)
                .HasColumnType("character varying")
                .HasColumnName("theoryContentHtml");
            entity.Property(e => e.TheoryContentJson)
                .HasColumnType("character varying")
                .HasColumnName("theoryContentJson");
            entity.Property(e => e.TheoryDescription)
                .HasMaxLength(1000)
                .HasColumnName("theoryDescription");
            entity.Property(e => e.TheoryName)
                .HasMaxLength(300)
                .HasColumnName("theoryName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Theories)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Theory_lessonId_fkey");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Enrollment_pkey");

            entity.ToTable("Enrollment");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.BaseUserId).HasColumnName("baseUserId");
            entity.Property(e => e.SubjectCurriculumId).HasColumnName("subjectCurriculumId");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.SubjectCurriculum).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.SubjectCurriculumId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Enrollment_subjectCurriculumId_fkey");
        });

        modelBuilder.Entity<EnrollmentProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EnrollmentProgress_pkey");

            entity.ToTable("EnrollmentProgress");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.EnrollmentId).HasColumnName("enrollmentId");
            entity.Property(e => e.LessonId).HasColumnName("lessonId");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.EnrollmentProgresses)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("EnrollmentProgress_enrollmentId_fkey");

            entity.HasOne(d => d.Lesson).WithMany(d => d.EnrollmentProgresses)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("EnrollmentProgress_lessonId_fkey");
        });
        
        modelBuilder.Entity<Container>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Container_pkey");

			entity.ToTable("Container");
            
            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.UserId)
                .HasColumnName("userId");

			entity.Property(e => e.FlashcardId)
				.HasColumnName("flashcardId");

			entity.Property(e => e.ViewAt)
			    .HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp with time zone")
                .HasColumnName("viewAt");

            entity.Property(e => e.ShuffleFlashcards)
                .HasColumnName("shuffleFlashcards");

            entity.Property(e => e.LearnRound)
                .HasColumnName("learnRound");
            
            entity.Property(e => e.LearnMode)
                .HasMaxLength(50)
                .HasColumnName("learnMode")
                .HasConversion(
                    v => v.ToString(),
                    v => (LearnMode)Enum.Parse(typeof(LearnMode), v));
            
            entity.Property(e => e.ShuffleLearn)
                .HasColumnName("shuffleLearn");

            entity.Property(e => e.StudyStarred)
                .HasColumnName("studyStarred");

            entity.Property(e => e.AnswerWith)
                .HasMaxLength(50)
                .HasColumnName("answerWith")
                .HasConversion(
                    v => v.ToString(),
                    v => (StudySetAnswerMode)Enum.Parse(typeof(StudySetAnswerMode), v));
            
            entity.Property(e => e.MultipleAnswerMode)
                .HasMaxLength(50)
                .HasColumnName("multipleAnswerMode")
                .HasConversion(
                    v => v.ToString(),
                    v => (MultipleAnswerMode)Enum.Parse(typeof(MultipleAnswerMode), v));
            
            entity.Property(e => e.CardsAnswerWith)
                .HasMaxLength(50)
                .HasColumnName("cardsAnswerWith")
                .HasConversion(
                    v => v.ToString(),
                    v => (LimitedStudySetAnswerMode)Enum.Parse(typeof(LimitedStudySetAnswerMode), v));

            entity.Property(e => e.ExtendedFeedbackBank)
                .HasColumnName("extendedFeedbackBank");

            entity.Property(e => e.EnableCardsSorting)
                .HasColumnName("enableCardsSorting");
            
            entity.Property(e => e.CardsRound)
                .HasColumnName("cardsRound");
            
            entity.Property(e => e.CardsStudyStarred)
                .HasColumnName("cardsStudyStarred");
            
            entity.Property(e => e.MatchStudyStarred)
                .HasColumnName("matchStudyStarred");

            entity.Property(e => e.Retrievability)
                .HasColumnName("retrievability")
                .HasDefaultValue(0.0);

            entity.Property(e => e.FsrsParameters)
                .HasColumnName("fsrsParameters")
                .HasDefaultValue(new double[19]);

            entity.Property(e => e.CardsPerDay)
                 .HasColumnName("cardsPerDay")
                 .HasDefaultValue(20);

            entity.HasMany(c => c.StudiableTerms)
                .WithOne(s => s.Container)
                .HasForeignKey(s => s.ContainerId) // Assuming StudiableTerm has ContainerId
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.StarredTerms)
                .WithOne(p => p.Container)
                .HasForeignKey(d => d.ContainerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.UserId, e.FlashcardId })
                .IsUnique()
                .HasDatabaseName("IX_Container_UserId_FlashcardId");
        });
        
        modelBuilder.Entity<StarredTerm>(entity =>
        {
            entity.ToTable("StarredTerm");

			// Composite primary key
			entity.HasKey(e => new { e.UserId, e.FlashcardContentId });

			entity.Property(e => e.UserId)
                .IsRequired(true)
                .HasColumnName("userId");

			entity.Property(e => e.FlashcardContentId)
			    .IsRequired(true)
				.HasColumnName("flashcardContentId");

			entity.Property(e => e.ContainerId)
				.HasColumnName("containerId");

			entity.HasOne(d => d.Container)
				.WithMany(p => p.StarredTerms)
				.HasForeignKey(d => d.ContainerId)
				.HasConstraintName("fk_starredTerm_container")
				.OnDelete(DeleteBehavior.Cascade);


			entity.HasOne(d => d.FlashcardContent)
				.WithMany(p => p.StarredTerm)
				.HasForeignKey(d => d.FlashcardContentId)
				.HasConstraintName("fk_starredTerm_flashcardContent")
				.OnDelete(DeleteBehavior.Cascade);
		});

        modelBuilder.Entity<StudiableTerm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StudiableTerm_pkey");

            entity.ToTable("StudiableTerm");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Mode)
                .HasColumnName("mode");

            entity.Property(e => e.UserId)
                .IsRequired(true)
                .HasColumnName("userId");

            entity.Property(e => e.ContainerId)
                .IsRequired(true)
                .HasColumnName("containerId");

            entity.Property(e => e.FlashcardContentId)
                .IsRequired(true)
                .HasColumnName("flashcardContentId");
            
            entity.HasOne(d => d.Container)
                .WithMany(p => p.StudiableTerms)
                .HasForeignKey(d => d.ContainerId)
                .HasConstraintName("fk_studiableTerm_container")
                .OnDelete(DeleteBehavior.Cascade); 

            
            entity.HasOne(d => d.FlashcardContent)
                .WithMany(p => p.StudiableTerm)
                .HasForeignKey(d => d.FlashcardContentId)
                .HasConstraintName("fk_studiableTerm_flashcardContent")
                .OnDelete(DeleteBehavior.Cascade);
            
        });


        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Question_pkey");

            entity.ToTable("Question");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.QuestionContent)
                .IsRequired()
                .HasColumnName("questionContent");

            entity.Property(e => e.LessonId)
                .HasColumnName("lessonId");

            entity.Property(e => e.ChapterId)
                .HasColumnName("chapterId");

            entity.Property(e => e.SubjectCurriculumId)
                .HasColumnName("subjectCurriculumId");

            entity.Property(e => e.SubjectId)
                .HasColumnName("subjectId");

            entity.Property(e => e.Difficulty)
                .HasMaxLength(50)
                .HasColumnName("difficulty")
                .HasConversion(
                    v => v.ToString(),
                    v => (Difficulty)Enum.Parse(typeof(Difficulty), v));

            entity.Property(e => e.QuestionType)
                .HasMaxLength(50)
                .HasColumnName("questionType")
                .HasConversion(
                    v => v.ToString(),
                    v => (QuestionType)Enum.Parse(typeof(QuestionType), v));

            entity.Property(e => e.CreatedBy)
                .HasColumnName("createdBy");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updatedBy");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            // Quan hệ với bảng Lesson
            entity.HasOne(d => d.Lesson)
                .WithMany(p => p.Questions)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("fk_question_lesson")
                .OnDelete(DeleteBehavior.Cascade); // Tùy chọn, có thể dùng Cascade nếu muốn

            // Quan hệ với bảng Chapter
            entity.HasOne(d => d.Chapter)
                .WithMany(p => p.Questions)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("fk_question_chapter")
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với bảng SubjectCurriculum
            entity.HasOne(d => d.SubjectCurriculum)
                .WithMany(p => p.Questions)
                .HasForeignKey(d => d.SubjectCurriculumId)
                .HasConstraintName("fk_question_subject_curriculum")
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với bảng Subject
            entity.HasOne(d => d.Subject)
                .WithMany(p => p.Questions)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("fk_question_subject")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionAnswer_pkey");

            entity.ToTable("QuestionAnswer");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.QuestionId)
                .IsRequired()
                .HasColumnName("questionId");

            entity.Property(e => e.AnswerContent)
                .IsRequired()
                .HasColumnName("answerContent");

            entity.Property(e => e.IsCorrectAnswer)
                .IsRequired()
                .HasColumnName("isCorrectAnswer");

            entity.Property(e => e.CreatedBy)
                .HasColumnName("createdBy");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updatedBy");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Question)
                .WithMany(p => p.QuestionAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("fk_question_answer_question")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserQuizProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserQuizProgress_pkey");

            entity.ToTable("UserQuizProgress");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("userId");

            entity.Property(e => e.LessonId)
                .HasColumnName("lessonId");

            entity.Property(e => e.ChapterId)
                .HasColumnName("chapterId");

            entity.Property(e => e.SubjectCurriculumId)
                .HasColumnName("subjectCurriculumId");

            entity.Property(e => e.SubjectId)
                .HasColumnName("subjectId");

            entity.Property(e => e.QuestionIds)
                .HasColumnName("questionIds");

            entity.Property(e => e.RecognizingQuestionQuantity)
                .IsRequired()
                .HasColumnName("recognizingQuestionQuantity");

            entity.Property(e => e.RecognizingPercent)
                .HasColumnName("recognizingPercent");

            entity.Property(e => e.ComprehensingQuestionQuantity)
                .IsRequired()
                .HasColumnName("comprehensingQuestionQuantity");

            entity.Property(e => e.ComprehensingPercent)
                .HasColumnName("comprehensingPercent");

            entity.Property(e => e.LowLevelApplicationQuestionQuantity)
                .IsRequired()
                .HasColumnName("lowLevelApplicationQuestionQuantity");

            entity.Property(e => e.LowLevelApplicationPercent)
                .HasColumnName("lowLevelApplicationPercent");

            entity.Property(e => e.HighLevelApplicationQuestionQuantity)
                .IsRequired()
                .HasColumnName("highLevelApplicationQuestionQuantity");

            entity.Property(e => e.HighLevelApplicationPercent)
                .HasColumnName("highLevelApplicationPercent");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            // Quan hệ với bảng Lesson
            entity.HasOne(d => d.Lesson)
                .WithMany(p => p.UserQuizProgresses)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("fk_user_quiz_progress_lesson")
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với bảng Chapter
            entity.HasOne(d => d.Chapter)
                .WithMany(p => p.UserQuizProgresses)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("fk_user_quiz_progress_chapter")
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với bảng SubjectCurriculum
            entity.HasOne(d => d.SubjectCurriculum)
                .WithMany(p => p.UserQuizProgresses)
                .HasForeignKey(d => d.SubjectCurriculumId)
                .HasConstraintName("fk_user_quiz_progress_subject_curriculum")
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với bảng Subject
            entity.HasOne(d => d.Subject)
                .WithMany(p => p.UserQuizProgresses)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("fk_user_quiz_progress_subject")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Cấu hình cho Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tag_pkey");

            entity.ToTable("Tag");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Name");

            entity.Property(e => e.NormalizedName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("NormalizedName");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("CreatedAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("UpdatedAt");

            entity.HasIndex(e => e.NormalizedName).HasDatabaseName("idx_tag_normalized_name");
        });

        // Cấu hình cho FlashcardTag
        modelBuilder.Entity<FlashcardTag>(entity =>
        {
            entity.HasKey(e => new { e.FlashcardId, e.TagId }).HasName("FlashcardTag_pkey");

            entity.ToTable("FlashcardTag");

            entity.Property(e => e.FlashcardId)
                .HasColumnName("FlashcardId");

            entity.Property(e => e.TagId)
                .HasColumnName("TagId");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("CreatedAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("UpdatedAt");

            entity.HasOne(d => d.Flashcard)
                .WithMany(p => p.FlashcardTags)
                .HasForeignKey(d => d.FlashcardId)
                .HasConstraintName("fk_flashcard");

            entity.HasOne(d => d.Tag)
                .WithMany(p => p.FlashcardTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("fk_tag");
        });

        modelBuilder.HasDbFunction(
            methodInfo: typeof(DocumentDbContext).GetMethod(nameof(NormalizeVietnameseDb), new[] { typeof(string) }),
            builderAction: x => x.HasName("normalize_vietnamese")
        );      

        modelBuilder.Entity<FSRSPreset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FSRSPreset_pkey");

            entity.ToTable("FSRSPreset");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("userId");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("title");

            entity.Property(e => e.FsrsParameters)
                .HasColumnName("fsrsParameters")
                .HasDefaultValue(new double[19]);

            entity.Property(e => e.Retrievability)
                .HasColumnName("retrievability")
                .HasDefaultValue(0.0);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.Property(e => e.IsPublicPreset)
                .HasColumnName("isPublicPreset")
                .HasDefaultValue(false);

            // Tạo index
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_FSRSPreset_UserId");
            entity.HasIndex(e => e.IsPublicPreset).HasDatabaseName("IX_FSRSPreset_IsPublicPreset");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    private static string ConvertGradeToString(DocumentGrade grade)
    {
        return grade.ToString();
    }

    private static DocumentGrade ConvertStringToGrade(string gradeString)
    {
        return Enum.TryParse<DocumentGrade>(gradeString, out var parsedGrade) ? parsedGrade : default;
    }

    public string NormalizeVietnameseDb(string input) => throw new NotSupportedException();
}
