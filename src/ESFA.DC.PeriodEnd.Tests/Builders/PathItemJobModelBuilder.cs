using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Tests.Builders
{
    internal class PathItemJobModelBuilder
    {
        private int _status;

        public PathItemJobModelBuilder WithCompletedStatus()
        {
            _status = (int)JobStatusType.Completed;
            return this;
        }

        public PathItemJobModelBuilder WithReadyStatus()
        {
            _status = (int)JobStatusType.Ready;
            return this;
        }

        public PathItemJobModelBuilder WithFailedStatus()
        {
            _status = (int)JobStatusType.Failed;
            return this;
        }

        public PathItemJobModelBuilder WithFailedRetryStatus()
        {
            _status = (int)JobStatusType.FailedRetry;
            return this;
        }

        public PathItemJobModel Build()
        {
            return new PathItemJobModel
            {
                Status = _status
            };
        }
    }
}
