using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class Filter
    {
        public Filter(
            string projectName,
            IEnumerable<DueFilterType> dueFilters,
            PreservationStatus? preservationStatus,
            IEnumerable<int> requirementTypeIds,
            IEnumerable<string> disciplineCodes,
            IEnumerable<int> responsibleIds,
            IEnumerable<string> tagFunctionCodes,
            IEnumerable<int> modeIds,
            IEnumerable<int> journeyIds,
            IEnumerable<int> stepIds,
            string tagNoStartsWith, 
            string commPkgNoStartsWith,
            string mcPkgNoStartsWith,
            string purchaseOrderNoStartsWith,
            string callOffStartsWith)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }
            ProjectName = projectName;
            DueFilters = dueFilters ?? new List<DueFilterType>();
            PreservationStatus = preservationStatus;
            RequirementTypeIds = requirementTypeIds ?? new List<int>();
            DisciplineCodes = disciplineCodes ?? new List<string>();
            ResponsibleIds = responsibleIds ?? new List<int>();
            TagFunctionCodes = tagFunctionCodes ?? new List<string>();
            ModeIds = modeIds ?? new List<int>();
            JourneyIds = journeyIds ?? new List<int>();
            StepIds = stepIds ?? new List<int>();
            TagNoStartsWith = tagNoStartsWith;
            McPkgNoStartsWith = mcPkgNoStartsWith;
            CallOffStartsWith = callOffStartsWith;
            PurchaseOrderNoStartsWith = purchaseOrderNoStartsWith;
            CommPkgNoStartsWith = commPkgNoStartsWith;
        }

        public string ProjectName { get; }
        public IEnumerable<DueFilterType> DueFilters { get; }
        public PreservationStatus? PreservationStatus { get;  }
        public IEnumerable<int> RequirementTypeIds { get; }
        public IEnumerable<string> DisciplineCodes { get; }
        public IEnumerable<int> ResponsibleIds { get; }
        public IEnumerable<string> TagFunctionCodes { get; }
        public IEnumerable<int> ModeIds { get; }
        public IEnumerable<int> JourneyIds { get; }
        public IEnumerable<int> StepIds { get; }
        public string TagNoStartsWith { get; }
        public string CommPkgNoStartsWith { get; }
        public string McPkgNoStartsWith { get; }
        public string CallOffStartsWith { get; }
        public string PurchaseOrderNoStartsWith { get; }
    }
}
