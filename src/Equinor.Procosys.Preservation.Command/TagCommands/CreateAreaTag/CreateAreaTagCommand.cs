using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommand : IRequest<Result<int>>
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
            string remark)
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
        }

        public string ProjectName { get; }
        public TagType TagType { get; }
        public string DisciplineCode { get; }
        public string AreaCode { get; }
        public string TagNoSuffix { get; }
        public int StepId { get; }
        public IEnumerable<Requirement> Requirements { get; }
        public string Description { get; }
        public string Remark { get; }
    }
}
