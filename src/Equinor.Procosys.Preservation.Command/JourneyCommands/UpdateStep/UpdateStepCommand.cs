﻿using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<Unit>>, IStepRequest
    {
        public UpdateStepCommand(int stepId, string title)
        {
            StepId = stepId;
            Title = title;
        }
        public int StepId { get; set; }
        public string Title { get; set; }
    }
}
