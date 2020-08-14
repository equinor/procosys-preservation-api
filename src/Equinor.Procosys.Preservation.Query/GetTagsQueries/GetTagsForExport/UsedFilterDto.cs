using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class UsedFilterDto
    {
        public UsedFilterDto(
            string projectName,
            string voidedFilter,
            IEnumerable<string> dueFilters, 
            string actionStatus,
            string preservationStatus,
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
        public string VoidedFilter { get; }
        public IEnumerable<string> DueFilters { get; }
        public string ActionStatus { get; }
        public string PreservationStatus { get; }
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
