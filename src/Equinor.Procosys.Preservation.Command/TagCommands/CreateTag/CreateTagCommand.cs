using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommand : IRequest<Result<List<int>>>
    {
        public CreateTagCommand(
            string plant,
            IEnumerable<string> tagNos,
            string projectName,
            int stepId,
            IEnumerable<Requirement> requirements,
            string remark,
            string storageArea)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagNos = tagNos ?? new List<string>();
            ProjectName = projectName;
            StepId = stepId;
            Requirements = requirements ?? new List<Requirement>();
            Remark = remark;
            StorageArea = storageArea;
        }

        public string Plant { get; }
        public IEnumerable<string> TagNos { get; }
        public string ProjectName { get; }
        public int StepId { get; }
        public IEnumerable<Requirement> Requirements { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
