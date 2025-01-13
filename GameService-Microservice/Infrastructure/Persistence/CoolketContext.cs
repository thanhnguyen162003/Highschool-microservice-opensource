using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public partial class CoolketContext : DbContext
{
    public CoolketContext()
    {
    }

    public CoolketContext(DbContextOptions<CoolketContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ket> Kets { get; set; }

    public virtual DbSet<KetContent> KetContents { get; set; }

    public virtual DbSet<HistoryPlay> HistoryPlays { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("User");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.DisplayName).HasColumnName("displayName");

            entity.Property(e => e.Avatar).HasColumnName("avatar");

            entity.Property(e => e.OwnerAvatar).HasColumnName("ownerAvatar");

            entity.Property(e => e.TotalPlay).HasColumnName("totalPlay");

            entity.Property(e => e.TotalHost).HasColumnName("totalHost");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.Property(e => e.Streak).HasColumnName("streak");

            entity.Property(e => e.HostKetIds).HasColumnName("hostKetIds");

            entity.Property(e => e.ParticipantKetIds).HasColumnName("participantKetIds");

        });

        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("avatar_pk");

            entity.ToTable("Avatar");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.Image).HasColumnName("image");

            entity.Property(e => e.Name)
                .HasColumnName("name");

            entity.Property(e => e.Rarity).HasColumnName("rarity");

            entity.Property(e => e.Type).HasColumnName("type");

            entity.Property(e => e.Background).HasColumnName("background");

        });

        modelBuilder.Entity<Ket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ket_pk");

            entity.ToTable("Ket");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            
            entity.Property(e => e.Thumbnail)
                .HasColumnType("character varying")
                .HasColumnName("thumbnail");
            
            entity.Property(e => e.TotalPlay).HasColumnName("totalPlay");
           
            entity.Property(e => e.TotalQuestion).HasColumnName("totalQuestion");

            entity.HasOne(d => d.Author).WithMany(u => u.Kets)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("ket_user_id_fk");
        });

        modelBuilder.Entity<KetContent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("KetContent");

            entity.Property(e => e.Answers)
                .HasColumnType("character varying")
                .HasColumnName("answers");
            entity.Property(e => e.CorrectAnswer)
                .HasColumnName("correctAnswer");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.KetId).HasColumnName("ketId");
            entity.Property(e => e.Question)
                .HasColumnType("character varying")
                .HasColumnName("question");

            entity.Property(e => e.TimeAnswer).HasColumnName("timeAnswer");

            entity.Property(e => e.Order).HasColumnName("order");

            entity.HasOne(d => d.Ket).WithMany(k => k.KetContents)
                .HasForeignKey(d => d.KetId)
                .HasConstraintName("ketcontent_ket_id_fk");
        });

        modelBuilder.Entity<HistoryPlay>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("HistoryPlay");

            entity.Property(e => e.Avatar)
                .HasColumnType("character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.DisplayName)
                .HasColumnType("character varying")
                .HasColumnName("displayName");
            entity.Property(e => e.Rank)
                .HasColumnType("character varying")
                .HasColumnName("rank");
            entity.Property(e => e.UserId).HasColumnName("playerId");
            entity.Property(e => e.TimeAverage).HasColumnName("timeAverage");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
