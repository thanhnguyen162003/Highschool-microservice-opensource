using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public partial class AcademyHubContext : DbContext
{
    public AcademyHubContext()
    {
    }

    public AcademyHubContext(DbContextOptions<AcademyHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<PendingZoneInvite> PendingZoneInvites { get; set; }

    public virtual DbSet<TestContent> Questions { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    public virtual DbSet<ZoneBan> ZoneBans { get; set; }

    public virtual DbSet<ZoneMembership> ZoneMemberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Assignment_pkey");

            entity.ToTable("Assignment");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AvailableAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("availableAt");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Noticed)
                .HasColumnType("character varying")
                .HasColumnName("noticed");
            entity.Property(e => e.DueAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("dueAt");
            entity.Property(e => e.LockedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("lockedAt");
            entity.Property(e => e.Published).HasColumnName("published");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.TotalQuestion).HasColumnName("totalQuestion");
            entity.Property(e => e.TotalTime).HasColumnName("totalTime");
            entity.Property(e => e.Type)
                .HasColumnType("character varying")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.ZoneId).HasColumnName("zoneId");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

            entity.HasOne(d => d.Zone).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Assignment_zoneId_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Group_pkey");

            entity.ToTable("Group");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Leader).HasColumnName("leader");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.TotalPeople).HasColumnName("totalPeople");
        });

        modelBuilder.Entity<PendingZoneInvite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PendingZoneInvite_pkey");

            entity.ToTable("PendingZoneInvite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("expiredAt");
          
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Type)
                .HasColumnType("character varying")
                .HasColumnName("type");
            entity.Property(e => e.ZoneId).HasColumnName("zoneId");
            entity.Property(e => e.InviteBy).HasColumnName("inviteBy");

            entity.HasOne(d => d.Zone).WithMany(p => p.PendingZoneInvites)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("PendingZoneInvite_zoneId_fkey");
        });

        modelBuilder.Entity<TestContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Question_pkey");

            entity.ToTable("TestContent");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Answers)
                .HasColumnType("character varying")
                .HasColumnName("answers");
            entity.Property(e => e.Assignmentid).HasColumnName("assignmentid");
            entity.Property(e => e.CorrectAnswer).HasColumnName("correctAnswer");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.Question)
                .HasColumnType("character varying")
                .HasColumnName("question");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Assignmentid)
                .HasConstraintName("Question_assignmentid_fkey");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Submission_pkey");

            entity.ToTable("Submission");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssignmentId).HasColumnName("assignmentId");
            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("Submission_assignmentId_fkey");

            entity.HasOne(d => d.Member).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("Submission_memberId_fkey");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Zone_pkey");

            entity.ToTable("Zone");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.BannerUrl)
                .HasColumnType("character varying")
                .HasColumnName("bannerUrl");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.LogoUrl)
                .HasColumnType("character varying")
                .HasColumnName("logoUrl");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.DocumentIds).HasColumnName("documentIds");
            entity.Property(e => e.FlashcardIds).HasColumnName("flashcardIds");
            entity.Property(e => e.FolderIds).HasColumnName("folderIds");

            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        modelBuilder.Entity<ZoneBan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ZoneBan_pkey");

            entity.ToTable("ZoneBan");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.UserId)
                .HasColumnType("character varying")
                .HasColumnName("userId");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

            entity.HasOne(d => d.Zone).WithMany(p => p.ZoneBans)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("ZoneBan_ZoneId_fkey");
        });

        modelBuilder.Entity<ZoneMembership>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ZoneMembership_pkey");

            entity.ToTable("ZoneMembership");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("deletedAt");
            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.Type)
                .HasColumnType("character varying")
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.ZoneId).HasColumnName("zoneId");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updatedAt");

            entity.Property(e => e.Email).HasColumnName("email");

            entity.Property(e => e.InviteBy).HasColumnName("inviteBy");

            entity.HasOne(d => d.Group).WithMany(p => p.ZoneMemberships)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("ZoneMembership_groupId_fkey");

            entity.HasOne(d => d.Zone).WithMany(p => p.ZoneMemberships)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("ZoneMembership_zoneId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
