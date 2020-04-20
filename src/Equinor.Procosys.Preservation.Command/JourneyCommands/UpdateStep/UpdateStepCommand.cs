﻿using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommand : IRequest<Result<int>>, IStepRequest
    {
        public UpdateStepCommand(int journeyId, int stepId, string title)
        {
            StepId = stepId;
            Title = title;
            JourneyId = journeyId;
        }
        
        public int StepId { get; set;}
        public string Title { get; set; }
        public int JourneyId { get; internal set; }
    }
}
