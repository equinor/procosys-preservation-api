using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class TagDto
    {
        public TagDto(
            int id,
            string areaCode,
            string calloffNo,
            string commPkgNo,
            string disciplineCode,
            RequirementDto firstUpcomingRequirement,
            bool isAreaTag,
            bool isVoided,
            string mcPkgNo, 
            string mode,
            bool readyToBePreserved,
            bool readyToBeBulkPreserved,
            string purchaseOrderNo,
            string remark,
            IEnumerable<RequirementDto> requirements,
            string responsibleCode,
            PreservationStatus status,
            string tagFunctionCode,
            string tagDescription,
            string tagNo)
        {
            Id = id;
            AreaCode = areaCode;
            CalloffNo = calloffNo;
            CommPkgNo = commPkgNo;
            Description = tagDescription;
            DisciplineCode = disciplineCode;
            FirstUpcomingRequirement = firstUpcomingRequirement;
            IsAreaTag = isAreaTag;
            IsVoided = isVoided;
            McPkgNo = mcPkgNo;
            Mode = mode;
            ReadyToBePreserved = readyToBePreserved;
            ReadyToBeBulkPreserved = readyToBeBulkPreserved;
            PurchaseOrderNo = purchaseOrderNo;
            Remark = remark;
            TagNo = tagNo;
            ResponsibleCode = responsibleCode;
            Status = status;
            TagFunctionCode = tagFunctionCode;
            Requirements = requirements ?? throw new ArgumentNullException(nameof(requirements));
        }

        public string AreaCode { get; }
        public string CalloffNo { get; }
        public string CommPkgNo { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public RequirementDto FirstUpcomingRequirement { get; }
        public int Id { get; }
        public bool IsAreaTag { get; }
        public bool IsVoided { get; }
        public string McPkgNo { get; }
        public string Mode { get; }
        public string PurchaseOrderNo { get; }
        public string Remark { get; }
        public bool ReadyToBePreserved { get; }
        public bool ReadyToBeBulkPreserved { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
        public PreservationStatus Status { get; }
        public string ResponsibleCode { get; }
        public string TagFunctionCode { get; }
        public string TagNo { get; }
    }
}
