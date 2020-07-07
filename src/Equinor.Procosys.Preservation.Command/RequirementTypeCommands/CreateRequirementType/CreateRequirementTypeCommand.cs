﻿using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType
{
    public class CreateRequirementTypeCommand : IRequest<Result<int>>
    {
        public CreateRequirementTypeCommand(int sortKey, string code, string title)
        {
            SortKey = sortKey;
            Code = code;
            Title = title;
        }

        public int SortKey { get; }
        public string Code { get; }
        public string Title { get; }
    }
}