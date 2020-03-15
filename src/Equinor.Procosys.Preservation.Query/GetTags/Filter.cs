using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class Filter
    {
        public ActionStatus? ActionStatus { get; set; }
        public IEnumerable<DueFilterType> DueFilters { get; set; } = new List<DueFilterType>();
        public PreservationStatus? PreservationStatus { get; set; }
        public IEnumerable<int> RequirementTypeIds { get; set; } = new List<int>();
        public IEnumerable<string> AreaCodes { get; set; } = new List<string>();
        public IEnumerable<string> DisciplineCodes { get; set; } = new List<string>();
        public IEnumerable<int> ResponsibleIds { get; set; } = new List<int>();
        public IEnumerable<string> TagFunctionCodes { get; set; } = new List<string>();
        public IEnumerable<int> ModeIds { get; set; } = new List<int>();
        public IEnumerable<int> JourneyIds { get; set; } = new List<int>();
        public IEnumerable<int> StepIds { get; set; } = new List<int>();
        public string TagNoStartsWith { get; set; }
        public string CommPkgNoStartsWith { get; set; }
        public string McPkgNoStartsWith { get; set; }
        public string CallOffStartsWith { get; set; }
        public string PurchaseOrderNoStartsWith { get; set; }
    }
}
