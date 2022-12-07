using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.FillPCSGuids
{
    public class FillPCSGuidsCommand : IRequest<Result<Unit>>
    {
        public FillPCSGuidsCommand(bool saveChanges) => SaveChanges = saveChanges;

        public bool SaveChanges { get; }
    }
}
