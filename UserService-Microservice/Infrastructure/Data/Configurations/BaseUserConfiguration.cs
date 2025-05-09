using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class BaseUserConfiguration : IEntityTypeConfiguration<BaseUser>
{
	public void Configure(EntityTypeBuilder<BaseUser> builder)
	{
		builder.Property(t => t.Username)
			.HasMaxLength(100)
			.IsRequired();
	}
}
