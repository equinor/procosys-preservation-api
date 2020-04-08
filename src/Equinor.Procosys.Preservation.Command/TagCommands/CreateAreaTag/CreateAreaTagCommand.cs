using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommand : AbstractAreaTag, IRequest<Result<int>>, IProjectRequest
    {
        public CreateAreaTagCommand(
            string projectName,
            TagType tagType,
            string disciplineCode,
            string areaCode,
            string tagNoSuffix,
            int stepId,
            IEnumerable<Requirement> requirements, 
            string description,
            string remark,
            string storageArea)
        {
            ProjectName = projectName;
            TagType = tagType;
            DisciplineCode = disciplineCode;
            AreaCode = areaCode;
            TagNoSuffix = tagNoSuffix;
            StepId = stepId;
            Requirements = requirements ?? new List<Requirement>();
            Description = description;
            Remark = remark;
            StorageArea = storageArea;
        }

        public string ProjectName { get; }
        public override TagType TagType { get; }
        public override string DisciplineCode { get; }
        public override string AreaCode { get; }
        public override string TagNoSuffix { get; }
        public int StepId { get; }
        public IEnumerable<Requirement> Requirements { get; }
        public string Description { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
