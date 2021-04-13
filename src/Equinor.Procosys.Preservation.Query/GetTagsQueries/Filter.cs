using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries
{
    public class Filter
    {
        public VoidedFilterType VoidedFilter { get; set; } = VoidedFilterType.NotVoided;
        public ActionStatus? ActionStatus { get; set; }
        public IList<DueFilterType> DueFilters { get; set; } = new List<DueFilterType>();
        public PreservationStatus? PreservationStatus { get; set; }
        public IList<int> RequirementTypeIds { get; set; } = new List<int>();
        public IList<string> AreaCodes { get; set; } = new List<string>();
        public IList<string> DisciplineCodes { get; set; } = new List<string>();
        public IList<int> ResponsibleIds { get; set; } = new List<int>();
        public IList<string> TagFunctionCodes { get; set; } = new List<string>();
        public IList<int> ModeIds { get; set; } = new List<int>();
        public IList<int> JourneyIds { get; set; } = new List<int>();
        public IList<int> StepIds { get; set; } = new List<int>();
        public string TagNoStartsWith { get; set; }
        public string CommPkgNoStartsWith { get; set; }
        public string McPkgNoStartsWith { get; set; }
        public string CallOffStartsWith { get; set; }
        public string PurchaseOrderNoStartsWith { get; set; }
        public string StorageAreaStartsWith { get; set; }
    }
}
