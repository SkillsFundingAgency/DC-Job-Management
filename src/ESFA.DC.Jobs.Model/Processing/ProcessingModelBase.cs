using System.Collections.Generic;

namespace ESFA.DC.Jobs.Model.Processing
{
    public abstract class ProcessingModelBase<TLookupModel>
        where TLookupModel : ProcessingLookupModelBase
    {
        protected ProcessingModelBase()
        {
            Jobs = new Dictionary<int, List<TLookupModel>>();
        }

        public Dictionary<int, List<TLookupModel>> Jobs { get; set; }
    }
}