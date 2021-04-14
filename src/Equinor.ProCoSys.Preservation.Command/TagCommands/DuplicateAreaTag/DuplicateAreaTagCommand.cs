using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.DuplicateAreaTag
{
    public class DuplicateAreaTagCommand : AbstractAreaTag, IRequest<Result<int>>, ITagCommandRequest
    {
        public DuplicateAreaTagCommand(
            int tagId,
            TagType tagType,
            string disciplineCode,
            string areaCode,
            string tagNoSuffix,
            string description,
            string remark,
            string storageArea)
        {
            TagId = tagId;
            TagType = tagType;
            DisciplineCode = disciplineCode;
            AreaCode = areaCode;
            TagNoSuffix = tagNoSuffix;
            Description = description;
            Remark = remark;
            StorageArea = storageArea;
        }

        public int TagId { get; }
        public override TagType TagType { get; }
        public override string DisciplineCode { get; }
        public override string AreaCode { get; }
        public override string PurchaseOrderCalloffCode => null;
        public override string TagNoSuffix { get; }
        public string Description { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
