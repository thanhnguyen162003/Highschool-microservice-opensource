using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dapper
{
	public class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
	{
		private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new ArgumentNullException("Connection string 'DefaultConnection' is not configured.");

        public IDbConnection Create()
		{
			return new NpgsqlConnection(_connectionString);
		}
	}
}
