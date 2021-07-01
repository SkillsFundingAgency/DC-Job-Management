using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Tests.Builders
{
    internal class PathPathItemsModelBuilder
    {
        private List<PathItemModel> _pathItems;

        public PathPathItemsModelBuilder WithPathItemModel(PathItemModel pathItemModel)
        {
            if (_pathItems == null)
            {
                _pathItems = new List<PathItemModel>();
            }

            _pathItems.Add(pathItemModel);

            return this;
        }

        public PathPathItemsModel Build()
        {
            return new PathPathItemsModel
            {
                PathItems = _pathItems
            };
        }
    }
}
