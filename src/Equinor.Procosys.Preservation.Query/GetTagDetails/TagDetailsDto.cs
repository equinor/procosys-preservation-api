using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class TagDetailsDto
    {
        public TagDetailsDto(int id,
            string tagNo,
            bool isVoided, 
            string description, 
            string status, 
            JourneyDetailsDto journey, 
            StepDetailsDto step, 
            string commPkgNo, 
            string mcPkgNo, 
            string calloffNo, 
            string purchaseOrderNo, 
            string areaCode, 
            TagType tagType, 
            bool readyToBePreserved, 
            string remark, 
            string storageArea, 
            string rowVersion)
        {
            Id = id;
            TagNo = tagNo;
            IsVoided = isVoided;
            Description = description;
            Status = status;
            Journey = journey;
            Step = step;
            CommPkgNo = commPkgNo;
            McPkgNo = mcPkgNo;
            CalloffNo = calloffNo;
            PurchaseOrderNo = purchaseOrderNo;
            AreaCode = areaCode;
            TagType = tagType;
            ReadyToBePreserved = readyToBePreserved;
            Remark = remark;
            StorageArea = storageArea;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string TagNo { get; }
        public bool IsVoided { get; }
        public string Description { get; }
        public string Status { get; }
        public JourneyDetailsDto Journey { get; }
        public StepDetailsDto Step { get; }
        public string CommPkgNo { get; }
        public string McPkgNo { get; }
        public string CalloffNo { get; }
        public string PurchaseOrderNo { get; }
        public string AreaCode { get; }
        public TagType TagType { get; }
        public bool ReadyToBePreserved { get; }
        public string Remark { get; }
        public string StorageArea { get; }
        public string RowVersion { get; }
    }
}
