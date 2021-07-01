using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager.Data
{
    public partial class JobQueueDataContext : IJobQueueDataContext
    {
        public async Task<IList<T>> FromSqlAsync<T>(CommandType commandType, string sql, object parameters)
        {
            using (var connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();

                return (await connection.QueryAsync<T>(sql, parameters, commandType: commandType)).ToList();
            }
        }
    }
}
