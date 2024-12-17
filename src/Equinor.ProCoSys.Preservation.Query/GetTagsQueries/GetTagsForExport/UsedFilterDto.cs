using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class UsedFilterDto
    {
        public UsedFilterDto(
            IEnumerable<string> actionStatus,
            IEnumerable<string> areaCodes,
            string callOffStartsWith,
            string commPkgNoStartsWith,
            IEnumerable<string> disciplineCodes,
            IEnumerable<string> dueFilters,
            IEnumerable<string> journeyTitles,
            string mcPkgNoStartsWith,
            IEnumerable<string> modeTitles,
            IEnumerable<string> preservationStatus,
            string projectDescription,
            string plant,
            string projectName,
            string purchaseOrderNoStartsWith,
            IEnumerable<string> requirementTypeTitles,
            IEnumerable<string> responsibleCodes,
            string storageAreaStartsWith,
            IEnumerable<string> tagFunctionCodes,
            string tagNoStartsWith,
            string voidedFilter)
        {
            ActionStatus = actionStatus;
            AreaCodes = areaCodes;
            CallOffStartsWith = callOffStartsWith;
            CommPkgNoStartsWith = commPkgNoStartsWith;
            DisciplineCodes = disciplineCodes;
            DueFilters = dueFilters;
            JourneyTitles = journeyTitles;
            McPkgNoStartsWith = mcPkgNoStartsWith;
            ModeTitles = modeTitles;
            PreservationStatus = preservationStatus;
            ProjectDescription = projectDescription;
            Plant = plant;
            ProjectName = projectName;
            PurchaseOrderNoStartsWith = purchaseOrderNoStartsWith;
            RequirementTypeTitles = requirementTypeTitles;
            ResponsibleCodes = responsibleCodes;
            StorageAreaStartsWith = storageAreaStartsWith;
            TagFunctionCodes = tagFunctionCodes;
            TagNoStartsWith = tagNoStartsWith;
            VoidedFilter = voidedFilter;
        }

        public IEnumerable<string> ActionStatus { get; }
        public IEnumerable<string> AreaCodes { get; }
        public string CallOffStartsWith { get; }
        public string CommPkgNoStartsWith { get; }
        public IEnumerable<string> DisciplineCodes { get; }
        public IEnumerable<string> DueFilters { get; }
        public IEnumerable<string> JourneyTitles { get; }
        public string McPkgNoStartsWith { get; }
        public IEnumerable<string> ModeTitles { get; }
        public string Plant { get; }
        public IEnumerable<string> PreservationStatus { get; }
        public string ProjectDescription { get; }
        public string ProjectName { get; }
        public string PurchaseOrderNoStartsWith { get; }
        public IEnumerable<string> RequirementTypeTitles { get; }
        public IEnumerable<string> ResponsibleCodes { get; }
        public string StorageAreaStartsWith { get; }
        public IEnumerable<string> TagFunctionCodes { get; }
        public string TagNoStartsWith { get; }
        public string VoidedFilter { get; }
    }
}
