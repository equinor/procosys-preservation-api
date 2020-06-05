using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommand : IRequest<Result<int>>
    {
        public CreateModeCommand(string title, bool forSupplier)
        {
            Title = title;
            ForSupplier = forSupplier;
        }

        public string Title { get; }
        public bool ForSupplier { get; }
    }
}
