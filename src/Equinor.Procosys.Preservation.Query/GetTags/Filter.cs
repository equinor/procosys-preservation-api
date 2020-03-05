using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class Filter
    {
        public Filter(
            string projectName,
            PreservationStatus? preservationStatus,
            IEnumerable<int> responsibleIds,
            IEnumerable<int> requirementTypeIds,
            IEnumerable<int> modeIds,
            IEnumerable<int> stepIds,
            string tagNo,
            string mcPkgNo,
            string callOff)
        {
            ProjectName = projectName;
            PreservationStatus = preservationStatus;
            ResponsibleIds = responsibleIds ?? new List<int>();
            RequirementTypeIds = requirementTypeIds ?? new List<int>();
            ModeIds = modeIds ?? new List<int>();
            StepIds = stepIds ?? new List<int>();
            TagNo = tagNo;
            McPkgNo = mcPkgNo;
            CallOff = callOff;
        }

        public string ProjectName { get; }
        public PreservationStatus? PreservationStatus { get;  }
        public IEnumerable<int> ResponsibleIds { get; }
        public IEnumerable<int> RequirementTypeIds { get; }
        public IEnumerable<int> ModeIds { get; }
        public IEnumerable<int> StepIds { get; }
        public string TagNo { get; }
        public string McPkgNo { get; }
        public string CallOff { get; }
    }
}
