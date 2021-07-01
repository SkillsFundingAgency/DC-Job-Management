using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.JobQueueManager.Audit
{
   public class AuditRepository : IAuditRepository
    {
        private const string _sql = "INSERT INTO Audit ([User], [TimeStampUTC], [OldValue], [NewValue], [Differentiator]) VALUES (@User, @TimeStampUTC, @OldValue, @NewValue, @Differentiator)";
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly string _auditConnectionString;

        public AuditRepository(IJsonSerializationService jsonSerializationService, string auditConnectionString)
        {
            _jsonSerializationService = jsonSerializationService;
            _auditConnectionString = auditConnectionString;
        }

        public async Task Save<T>(IAuditContext context, T before, T after, CancellationToken cancellationToken)
        {
            var beforeString = Serialise(before);
            var afterString = Serialise(after);
            if (beforeString != afterString)
            {
                using (var connection = new SqlConnection(_auditConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync(_sql, new { User = context.Username, TimeStampUTC = context.TimeStamp, OldValue = beforeString, NewValue = afterString, Differentiator = context.Differentiator });
                }
            }
        }

        private string Serialise<T>(T dto)
        {
            if (dto != null)
            {
               return _jsonSerializationService.Serialize(dto);
            }

            return null;
        }
    }
}
