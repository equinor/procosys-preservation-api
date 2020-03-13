using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTags;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class FilterDto
    {
        public string ProjectName { get; set; }
        public IEnumerable<DueFilterType> DueFilters { get; set; }
        public ActionStatus? ActionStatus { get; set; }
        public PreservationStatus? PreservationStatus { get; set; }
        public IEnumerable<int> RequirementTypeIds { get; set; }
        public IEnumerable<string> DisciplineCodes { get; set; }
        public IEnumerable<int> ResponsibleIds { get; set; }
        public IEnumerable<string> TagFunctionCodes { get; set; }
        public IEnumerable<int> ModeIds { get; set; }
        public IEnumerable<int> JourneyIds { get; set; }
        public IEnumerable<int> StepIds { get; set; }
        public string TagNoStartsWith { get; set; }
        public string CommPkgNoStartsWith { get; set; }
        public string McPkgNoStartsWith { get; set; }
        public string CallOffStartsWith { get; set; }
        public string PurchaseOrderNoStartsWith { get; set; }
    }
}
