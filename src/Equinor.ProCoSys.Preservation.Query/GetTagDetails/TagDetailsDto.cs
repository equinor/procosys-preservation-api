using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetTagDetails
{
    public class TagDetailsDto
    {
        public TagDetailsDto(int id,
            string tagNo,
            bool isInUse,
            bool isVoided,
            bool isVoidedInSource,
            bool isDeletedInSource,
            string description,
            string status,
            JourneyDetailsDto journey,
            StepDetailsDto step,
            ModeDetailsDto mode,
            ResponsibleDetailsDto responsible,
            string commPkgNo,
            string mcPkgNo,
            string calloffNo,
            string purchaseOrderNo,
            string areaCode,
            string disciplineCode,
            TagType tagType,
            bool readyToBePreserved,
            string remark,
            string storageArea,
            string rowVersion)
        {
            Id = id;
            TagNo = tagNo;
            IsInUse = isInUse;
            IsVoided = isVoided;
            IsVoidedInSource = isVoidedInSource;
            IsDeletedInSource = isDeletedInSource;
            Description = description;
            Status = status;
            Journey = journey;
            Step = step;
            Mode = mode;
            Responsible = responsible;
            CommPkgNo = commPkgNo;
            McPkgNo = mcPkgNo;
            CalloffNo = calloffNo;
            PurchaseOrderNo = purchaseOrderNo;
            AreaCode = areaCode;
            DisciplineCode = disciplineCode;
            TagType = tagType;
            ReadyToBePreserved = readyToBePreserved;
            Remark = remark;
            StorageArea = storageArea;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string TagNo { get; }
        public bool IsInUse { get; }
        public bool IsVoided { get; }
        public bool IsVoidedInSource { get; }
        public bool IsDeletedInSource { get; }
        public string Description { get; }
        public string Status { get; }
        public JourneyDetailsDto Journey { get; }
        public StepDetailsDto Step { get; }
        public ModeDetailsDto Mode { get; }
        public ResponsibleDetailsDto Responsible { get; }
        public string CommPkgNo { get; }
        public string McPkgNo { get; }
        public string CalloffNo { get; }
        public string PurchaseOrderNo { get; }
        public string AreaCode { get; }
        public string DisciplineCode { get; }
        public TagType TagType { get; }
        public bool ReadyToBePreserved { get; }
        public string Remark { get; }
        public string StorageArea { get; }
        public string RowVersion { get; }
    }
}
