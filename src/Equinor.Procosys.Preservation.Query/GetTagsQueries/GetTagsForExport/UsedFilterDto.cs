using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class UsedFilterDto
    {
        public UsedFilterDto(
            string projectName,
            VoidedFilterType? voidedFilter,
            IEnumerable<DueFilterType> dueFilters, 
            ActionStatus? actionStatus,
            PreservationStatus? preservationStatus,
            IEnumerable<string> requirementTypeTitles,
            IEnumerable<string> areaCodes,
            IEnumerable<string> disciplineCodes,
            IEnumerable<string> responsibleCodes,
            IEnumerable<string> tagFunctionCodes,
            IEnumerable<string> modeTitles,
            IEnumerable<string> journeyTitles,
            IEnumerable<string> stepTitles,
            string tagNoStartsWith,
            string commPkgNoStartsWith,
            string mcPkgNoStartsWith,
            string callOffStartsWith,
            string purchaseOrderNoStartsWith,
            string storageAreaStartsWith)
        {
            ProjectName = projectName;
            VoidedFilter = voidedFilter;
            DueFilters = dueFilters;
            ActionStatus = actionStatus;
            PreservationStatus = preservationStatus;
            RequirementTypeTitles = requirementTypeTitles;
            AreaCodes = areaCodes;
            DisciplineCodes = disciplineCodes;
            ResponsibleCodes = responsibleCodes;
            TagFunctionCodes = tagFunctionCodes;
            ModeTitles = modeTitles;
            JourneyTitles = journeyTitles;
            StepTitles = stepTitles;
            TagNoStartsWith = tagNoStartsWith;
            CommPkgNoStartsWith = commPkgNoStartsWith;
            McPkgNoStartsWith = mcPkgNoStartsWith;
            CallOffStartsWith = callOffStartsWith;
            PurchaseOrderNoStartsWith = purchaseOrderNoStartsWith;
            StorageAreaStartsWith = storageAreaStartsWith;
        }

        public string ProjectName { get; }
        public VoidedFilterType? VoidedFilter { get; }
        public IEnumerable<DueFilterType> DueFilters { get; }
        public ActionStatus? ActionStatus { get; }
        public PreservationStatus? PreservationStatus { get; }
        public IEnumerable<string> RequirementTypeTitles { get; }
        public IEnumerable<string> AreaCodes { get; }
        public IEnumerable<string> DisciplineCodes { get; }
        public IEnumerable<string> ResponsibleCodes { get; }
        public IEnumerable<string> TagFunctionCodes { get; }
        public IEnumerable<string> ModeTitles { get; }
        public IEnumerable<string> JourneyTitles { get; }
        public IEnumerable<string> StepTitles { get; }
        public string TagNoStartsWith { get; }
        public string CommPkgNoStartsWith { get; }
        public string McPkgNoStartsWith { get; }
        public string CallOffStartsWith { get; }
        public string PurchaseOrderNoStartsWith { get; }
        public string StorageAreaStartsWith { get; }
    }
}
