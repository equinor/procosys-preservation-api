﻿using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommand : AbstractAreaTag, IRequest<Result<int>>, IProjectRequest
    {
        public CreateAreaTagCommand(
            string projectName,
            TagType tagType,
            string disciplineCode,
            string areaCode,
            string purchaseOrderCalloffCode,
            string tagNoSuffix,
            int stepId,
            IEnumerable<RequirementForCommand> requirements, 
            string description,
            string remark,
            string storageArea)
        {
            ProjectName = projectName;
            TagType = tagType;
            DisciplineCode = disciplineCode;
            PurchaseOrderCalloffCode = purchaseOrderCalloffCode;
            AreaCode = areaCode;
            TagNoSuffix = tagNoSuffix;
            StepId = stepId;
            Requirements = requirements ?? new List<RequirementForCommand>();
            Description = description;
            Remark = remark;
            StorageArea = storageArea;
        }

        public string ProjectName { get; }
        public override TagType TagType { get; }
        public override string DisciplineCode { get; }
        public override string AreaCode { get; }
        public override string PurchaseOrderCalloffCode { get; }
        public override string TagNoSuffix { get; }
        public int StepId { get; }
        public IEnumerable<RequirementForCommand> Requirements { get; }
        public string Description { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
