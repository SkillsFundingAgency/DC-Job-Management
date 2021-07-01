using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Tests.Builders
{
    internal class PathItemModelBuilder
    {
        private List<PathItemJobModel> _pathItemJobModels;
        private bool _isProviderReport;

        public PathItemModelBuilder WithPathItemJobModel(PathItemJobModel pathItemModel)
        {
            if (_pathItemJobModels == null)
            {
                _pathItemJobModels = new List<PathItemJobModel>();
            }

            _pathItemJobModels.Add(pathItemModel);

            return this;
        }

        public PathItemModelBuilder WithEmptyPathItemJobModelCollection()
        {
            _pathItemJobModels = new List<PathItemJobModel>();
            return this;
        }

        public PathItemModelBuilder WithIsProviderReport()
        {
            _isProviderReport = true;
            return this;
        }

        public PathItemModel Build()
        {
            return new PathItemModel
            {
                PathItemJobs = _pathItemJobModels,
                IsProviderReport = _isProviderReport
            };
        }
    }
}
