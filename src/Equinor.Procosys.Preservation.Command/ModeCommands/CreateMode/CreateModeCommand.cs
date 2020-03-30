using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommand : IRequest<Result<int>>
    {
        public CreateModeCommand(string plant, string title)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Title = title;
        }

        public string Plant { get; }
        public string Title { get; }
    }
}
