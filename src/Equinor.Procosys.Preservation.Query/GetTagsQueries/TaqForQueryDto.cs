using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries
{
    public class TaqForQueryDto
    {
        public string AreaCode { get; set; }
        public bool AnyOverDueActions { get; set; }
        public bool AnyOpenActions { get; set; }
        public bool AnyClosedActions { get; set; }
        public string Calloff { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public string DisciplineCode { get; set; }
        public bool IsVoided { get; set; }
        public int JourneyId { get; set; }
        public string McPkgNo { get; set; }
        public string ModeTitle { get; set; }
        public DateTime? NextDueTimeUtc { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string ResponsibleCode { get; set; }
        public string ResponsibleDescription { get; set; }
        public PreservationStatus Status { get; set; }
        public string StorageArea { get; set; }
        public int StepId { get; set; }
        public int TagId { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; set; }
        public TagType TagType { get; set; }
        public byte[] RowVersion { get; set; }
        public Journey JourneyWithSteps { get; set; }
        public Step NextStep { get; set; }

        public ActionStatus? GetActionStatus()
        {
            if (AnyOverDueActions)
            {
                return ActionStatus.HasOverDue;
            }
            if (AnyOpenActions)
            {
                return ActionStatus.HasOpen;
            }
            if (AnyClosedActions)
            {
                return ActionStatus.HasClosed;
            }

            return null;
        }
    }
}
