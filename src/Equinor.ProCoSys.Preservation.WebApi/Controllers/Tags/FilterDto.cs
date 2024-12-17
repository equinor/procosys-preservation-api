using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class FilterDto
    {
        public string ProjectName { get; set; }
        public VoidedFilterType? VoidedFilter { get; set; }
        public IEnumerable<DueFilterType> DueFilters { get; set; }
        public IEnumerable<ActionStatus> ActionStatus { get; set; }
        public IEnumerable<PreservationStatus> PreservationStatus { get; set; }
        public IEnumerable<int> RequirementTypeIds { get; set; }
        public IEnumerable<string> AreaCodes { get; set; }
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
        public string StorageAreaStartsWith { get; set; }
    }
}
