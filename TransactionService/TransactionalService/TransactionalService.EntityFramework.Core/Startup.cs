using Furion;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionalService.EntityFramework.Core.DbContexts;

namespace TransactionalService.EntityFramework.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(WebApplicationBuilder builder)
        {
			builder.Services.AddDbContext<TransactionDbContext>(options =>
			{
				options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
					opt =>
					{
						opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
						opt.UseRelationalNulls();
					});
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			});
		}
    }
}
