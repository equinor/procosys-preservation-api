using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.DuplicateAreaTag
{
    public class DuplicateAreaTagCommand : IRequest<Result<int>>, ITagCommandRequest
    {
        public DuplicateAreaTagCommand(
            int tagId,
            string disciplineCode,
            string areaCode,
            string tagNoSuffix,
            string description,
            string remark,
            string storageArea)
        {
            TagId = tagId;
            DisciplineCode = disciplineCode;
            AreaCode = areaCode;
            TagNoSuffix = tagNoSuffix;
            Description = description;
            Remark = remark;
            StorageArea = storageArea;
        }

        public int TagId { get; }
        public string DisciplineCode { get; }
        public string AreaCode { get; }
        public string TagNoSuffix { get; }
        public string Description { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
