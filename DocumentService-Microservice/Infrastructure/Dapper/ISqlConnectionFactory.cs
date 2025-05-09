using System.Data;

namespace Infrastructure.Dapper
{
    public interface ISqlConnectionFactory
    {
        IDbConnection Create();
	}
}
