using Domain.Entities;
using Domain.Enumerations;

namespace Infrastructure.Data;

public partial class UserDatabaseContext : DbContext
{
	public UserDatabaseContext()
	{
	}

	public UserDatabaseContext(DbContextOptions<UserDatabaseContext> options)
		: base(options)
	{
	}

	public DbSet<OutboxMessage> OutboxMessages { get; set; }
	public virtual DbSet<BaseUser> BaseUsers { get; set; }

	public virtual DbSet<Certificate> Certificates { get; set; }

	public virtual DbSet<Enrollment> Enrollments { get; set; }

	public virtual DbSet<EnrollmentProcess> EnrollmentProcesses { get; set; }

	public virtual DbSet<Note> Notes { get; set; }

	public virtual DbSet<RecentView> RecentViews { get; set; }

	public virtual DbSet<Report> Reports { get; set; }

	public virtual DbSet<ReportDocument> ReportDocuments { get; set; }

	public virtual DbSet<Role> Roles { get; set; }

	public virtual DbSet<ChosenSubjectCurriculum> ChosenSubjectCurricula { get; set; }

	public virtual DbSet<Student> Students { get; set; }

	public virtual DbSet<Teacher> Teachers { get; set; }

	public virtual DbSet<ImageReport> ImageReports { get; set; }

	public virtual DbSet<UserSubject> UserSubjects { get; set; }

	public virtual DbSet<Roadmap> Roadmaps { get; set; }

	public virtual DbSet<Session> Sesssions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

		modelBuilder.Entity<OutboxMessage>(entity =>
		{
			entity.HasKey(e => e.EventId);

			entity.ToTable("OutboxMessage");

			entity.Property(e => e.EventId)
				.ValueGeneratedNever()
				.HasColumnName("eventId");

			entity.Property(e => e.EventPayload).HasColumnName("eventPayload");
			entity.Property(e => e.OccurredOn).HasColumnName("occurredOn");
			entity.Property(e => e.ProcessedOn).HasColumnName("processedOn");
			entity.Property(e => e.IsMessageDispatched).HasColumnName("isMessageDispatched");
			entity.Property(e => e.Error).HasColumnName("error");

		});
		modelBuilder.Entity<BaseUser>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("BaseUser_pkey");

			entity.ToTable("BaseUser");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.Bio)
				.HasMaxLength(255)
				.HasColumnName("bio");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
			entity.Property(e => e.DeletedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("deletedAt");
			entity.Property(e => e.Email)
				.HasMaxLength(100)
				.HasColumnName("email");
			entity.Property(e => e.Fullname)
				.HasMaxLength(100)
				.HasColumnName("fullname");
			entity.Property(e => e.LastLoginAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("lastLoginAt");
			entity.Property(e => e.Password).HasColumnName("password");
			entity.Property(e => e.PasswordSalt).HasColumnName("passwordSalt");
			entity.Property(e => e.ProfilePicture)
				.HasMaxLength(255)
				.HasColumnName("profilePicture");
			entity.Property(e => e.Provider)
				.HasMaxLength(50)
				.HasColumnName("provider");
			entity.Property(e => e.RoleId).HasColumnName("roleId");
			entity.Property(e => e.Status)
				.HasMaxLength(20)
				.HasColumnName("status");
			entity.Property(e => e.Timezone)
				.HasMaxLength(50)
				.HasColumnName("timezone");
			entity.Property(e => e.UpdatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("updatedAt");
			entity.Property(e => e.Username)
				.HasMaxLength(50)
				.HasColumnName("username");
			entity.Property(e => e.ProgressStage)
				.HasColumnName("progressStage");
			entity.Property(e => e.Address)
				.HasColumnName("address");

			entity.Property(e => e.Birthdate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("birthdate");

            entity.HasOne(d => d.Role).WithMany()
				.HasForeignKey(d => d.RoleId)
				.HasConstraintName("BaseUser_roleId_fkey");

			entity.HasOne(e => e.Student)
				.WithOne(e => e.BaseUser)
				.HasForeignKey<Student>(e => e.BaseUserId);

			entity.HasOne(e => e.Teacher)
				.WithOne(e => e.BaseUser)
				.HasForeignKey<Teacher>(e => e.BaseUserId);
		});

		modelBuilder.Entity<ChosenSubjectCurriculum>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("ChosenSubjectCurriculum_pkey");

			entity.ToTable("ChosenSubjectCurriculum");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.UserId)
				.HasColumnName("userId");
			entity.Property(e => e.SubjectId)
				.HasColumnName("subjectId");
			entity.Property(e => e.CurriculumId)
				.HasColumnName("curriculumId");
			entity.Property(e => e.SubjectCurriculumId)
				.HasColumnName("subjectCurriculumId");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
			entity.Property(e => e.UpdatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("updatedAt");
		});

		modelBuilder.Entity<Session>(entity =>
		{
			entity.HasKey(e => e.Id);

            entity.ToTable("Session");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

			entity.Property(e => e.UserId).HasColumnName("userId");

            entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

			entity.Property(e => e.DeviceInfo).HasColumnName("deviceInfo");

			entity.Property(e => e.RefreshToken).HasColumnName("refreshToken");

            entity.Property(e => e.ExpiredAt)
				.HasColumnType("timestamp without time zone")
                .HasColumnName("expiredAt");

            entity.Property(e => e.IsRevoked).HasColumnName("isRevoked");

			entity.Property(e => e.IpAddress).HasColumnName("ipAddress");

            entity.HasOne(d => d.User).WithMany()
				.HasForeignKey(d => d.UserId);
        });

		modelBuilder.Entity<Certificate>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Certificate_pkey");

			entity.ToTable("Certificate");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.CertLink)
				.HasMaxLength(255)
				.HasColumnName("certLink");
			entity.Property(e => e.CertName)
				.HasMaxLength(100)
				.HasColumnName("certName");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
			entity.Property(e => e.DeletedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("deletedAt");
			entity.Property(e => e.IssueDate)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("issueDate");
			entity.Property(e => e.IssuedBy)
				.HasMaxLength(100)
				.HasColumnName("issuedBy");
			entity.Property(e => e.TeacherId).HasColumnName("teacherId");
			entity.Property(e => e.UpdatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("updatedAt");

			entity.HasOne(d => d.Teacher).WithMany(p => p.Certificates)
				.HasForeignKey(d => d.TeacherId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("Certificate_teacherId_fkey");
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
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
			entity.Property(e => e.StudentId).HasColumnName("studentId");
			entity.Property(e => e.SubjectId).HasColumnName("subjectId");
			entity.Property(e => e.UpdatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("updatedAt");

			entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
				.HasForeignKey(d => d.StudentId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("Enrollment_studentId_fkey");
		});

		modelBuilder.Entity<EnrollmentProcess>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("EnrollmentProcess_pkey");

			entity.ToTable("EnrollmentProcess");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.ChapterId).HasColumnName("chapterId");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
			entity.Property(e => e.DeletedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("deletedAt");
			entity.Property(e => e.EnrollmentId).HasColumnName("enrollmentId");
			entity.Property(e => e.LessonId).HasColumnName("lessonId");
			entity.Property(e => e.UpdatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("updatedAt");

			entity.HasOne(d => d.Enrollment).WithMany(p => p.EnrollmentProcesses)
				.HasForeignKey(d => d.EnrollmentId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("EnrollmentProcess_enrollmentId_fkey");
		});

		modelBuilder.Entity<Note>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Note_pkey");

			entity.ToTable("Note");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
			entity.Property(e => e.DeletedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("deletedAt");
			entity.Property(e => e.NoteBody).HasColumnName("noteBody");
			entity.Property(e => e.NoteName)
				.HasMaxLength(100)
				.HasColumnName("noteName");
			entity.Property(e => e.UpdatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("updatedAt");
			entity.Property(e => e.UserId).HasColumnName("userId");

			entity.HasOne(d => d.User).WithMany(p => p.Notes)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("Note_userId_fkey");
		});

		modelBuilder.Entity<RecentView>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("RecentView_pkey");

			entity.ToTable("RecentView");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
			entity.Property(e => e.DeletedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("deletedAt");
			entity.Property(e => e.DiscussionId).HasColumnName("discussionId");
			entity.Property(e => e.FlashcardId).HasColumnName("flashcardId");
			entity.Property(e => e.SubjectId).HasColumnName("subjectId");
			entity.Property(e => e.UpdatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("updatedAt");
			entity.Property(e => e.UserId).HasColumnName("userId");

			entity.HasOne(d => d.User).WithMany(p => p.RecentViews)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("RecentView_userId_fkey");
		});

		modelBuilder.Entity<Report>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Report_pkey");

			entity.ToTable("Report");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.ReportContent).HasColumnName("reportContent");
			entity.Property(e => e.ReportTitle)
				.HasMaxLength(100)
				.HasColumnName("reportTitle");
			entity.Property(e => e.Status)
				.HasMaxLength(20)
				.HasColumnName("status");

			entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");

			entity.Property(e => e.UserId).HasColumnName("userId");

			entity.HasOne(d => d.User).WithMany(p => p.Reports)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("Report_userId_fkey");
		});

		modelBuilder.Entity<ReportDocument>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Report_document_pkey");

			entity.ToTable("ReportDocument");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");

			entity.Property(e => e.ReportContent).HasColumnName("reportContent");

			entity.Property(e => e.ReportTitle)
				.HasMaxLength(255)
				.HasColumnName("reportTitle");

			entity.Property(e => e.Status)
				.HasMaxLength(20)
				.HasColumnName("status");

			entity.Property(e => e.CreatedAt)
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");

			entity.Property(e => e.ReportType)
				.HasConversion(
					v => v.ToString(),
					v => (ReportType)Enum.Parse(typeof(ReportType), v))
				.HasColumnName("reportType");

			entity.Property(e => e.DocumentId).HasColumnName("documentId").IsRequired();

			entity.Property(e => e.UserId).HasColumnName("userId");
			 
			entity.HasOne(d => d.User).WithMany(p => p.ReportDocuments)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("Report_userId_fkey");
		});

		modelBuilder.Entity<ImageReport>(entity =>
		{
			entity.HasKey(e => new { e.ReportId, e.ImageUrl });

			entity.ToTable("ImageReport");

			entity.Property(e => e.ReportId).HasColumnName("reportId");

			entity.Property(e => e.ImageUrl).HasColumnName("url");

			entity.HasOne(d => d.Report).WithMany(p => p.ImageReports)
				.HasForeignKey(d => d.ReportId)
				.OnDelete(DeleteBehavior.ClientSetNull);
		});

		modelBuilder.Entity<Role>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Role_pkey");

			entity.ToTable("Role");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.RoleName)
				.HasMaxLength(50)
				.HasColumnName("roleName");
		});

		modelBuilder.HasPostgresEnum<MBTIType>();

		modelBuilder.Entity<Student>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Student_pkey");

			entity.ToTable("Student");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");

			entity.Property(e => e.BaseUserId).HasColumnName("baseUserId");

			entity.Property(e => e.Grade).HasColumnName("grade");

			entity.Property(e => e.SchoolName)
				.HasColumnName("schoolName");

			entity.Property(e => e.Major).HasColumnName("major");

			entity.Property(e => e.TypeExam).HasColumnName("typeExam");

            entity.Property(e => e.MbtiType)
				.HasConversion(
					v => v.ToString(),
					v => (MBTIType)Enum.Parse(typeof(MBTIType), v))
				.HasColumnName("mbtiType");

			entity.Property(e => e.HollandType).HasColumnName("hollandType");

			entity.Property(e => e.CardUrl).HasColumnName("CardUrl");

			entity.HasOne(d => d.BaseUser)
				.WithOne(d => d.Student)
				.HasForeignKey<Student>(d => d.BaseUserId);

		});


		modelBuilder.Entity<Teacher>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Teacher_pkey");

			entity.ToTable("Teacher");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.BaseUserId).HasColumnName("baseUserId");
			entity.Property(e => e.ContactNumber)
				.HasMaxLength(15)
				.HasColumnName("contactNumber");
			entity.Property(e => e.ExperienceYears).HasColumnName("experienceYears");
			entity.Property(e => e.GraduatedUniversity)
				.HasMaxLength(100)
				.HasColumnName("graduatedUniversity");
			entity.Property(e => e.Pin)
				.HasMaxLength(10)
				.HasColumnName("pin");
			entity.Property(e => e.Rating).HasColumnName("rating");
			entity.Property(e => e.SubjectsTaught)
				.HasMaxLength(255)
				.HasColumnName("subjectsTaught");
			entity.Property(e => e.Verified)
				.HasDefaultValue(false)
				.HasColumnName("verified");
			entity.Property(e => e.VideoIntroduction)
				.HasMaxLength(255)
				.HasColumnName("videoIntroduction");
			entity.Property(e => e.WorkPlace)
				.HasMaxLength(100)
				.HasColumnName("workPlace");

			entity.HasOne(d => d.BaseUser)
				.WithOne(d => d.Teacher)
				.HasForeignKey<Teacher>(d => d.BaseUserId);
		});

		modelBuilder.Entity<UserSubject>(entity =>
		{
			entity.HasKey(e => new { e.UserId, e.SubjectId });

			entity.ToTable("UserSubject");

			entity.Property(e => e.UserId).HasColumnName("userId");

			entity.Property(e => e.SubjectId).HasColumnName("subjectId");

			entity.HasOne(d => d.User).WithMany(p => p.UserSubjects)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull);
		});

		modelBuilder.Entity<Roadmap>(entity =>
		{
			entity.HasKey(e => e.UserId);

			entity.ToTable("Roadmap");

			entity.Property(e => e.Id).HasColumnName("id");

			entity.Property(e => e.Name).HasColumnName("name");

			entity.Property(e => e.Description).HasColumnName("description");

			entity.Property(e => e.ContentJson).HasColumnName("contentJson");

			entity.Property(e => e.SubjectId).HasColumnName("subjectId");

			entity.Property(e => e.DocumentId).HasColumnName("documentId");

			entity.Property(e => e.TypeExam).HasColumnName("typeExam");

			entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

			entity.Property(e => e.CreatedAt)
						.HasColumnType("timestamp without time zone")
						.HasColumnName("createdAt");

			entity.Property(e => e.UpdatedAt)
						.HasColumnType("timestamp without time zone")
						.HasColumnName("updatedAt");

			entity.Property(e => e.UserId).HasColumnName("userId");

			entity.HasOne(e => e.User)
				.WithOne(e => e.Roadmap)
				.HasForeignKey<Roadmap>(e => e.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull);

		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}