using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class TagDto
    {
        public TagDto(
            int id,
            string areaCode,
            string calloffNumber,
            string commPkgNumber,
            string disciplineCode,
            bool isAreaTag,
            bool isVoided,
            string mcPkgNumber,
            string projectNumber,
            string purchaseOrderNumber,
            IEnumerable<RequirementDto> requirements,
            PreservationStatus status,
            int stepId,
            string tagFunctionCode,
            string tagNo)
        {
            Id = id;
            AreaCode = areaCode;
            CalloffNumber = calloffNumber;
            CommPkgNumber = commPkgNumber;
            DisciplineCode = disciplineCode;
            IsAreaTag = isAreaTag;
            IsVoided = isVoided;
            McPkgNumber = mcPkgNumber;
            ProjectNumber = projectNumber;
            PurchaseOrderNumber = purchaseOrderNumber;
            TagNo = tagNo;
            Status = status;
            StepId = stepId;
            TagFunctionCode = tagFunctionCode;
            Requirements = requirements ?? throw new ArgumentNullException(nameof(requirements));
        }

        public string AreaCode { get; }
        public string CalloffNumber { get; }
        public string CommPkgNumber { get; }
        public string DisciplineCode { get; }
        public int Id { get; }
        public bool IsAreaTag { get; }
        public bool IsVoided { get; }
        public string McPkgNumber { get; }
        public string ProjectNumber { get; }
        public string PurchaseOrderNumber { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
        public PreservationStatus Status { get; }
        public int StepId { get; }
        public string TagFunctionCode { get; }
        public string TagNo { get; }
    }
}
