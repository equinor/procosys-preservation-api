using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.SyncTagData
{
    public class SyncTagDataCommand : IRequest<Result<Unit>>
    {
        public SyncTagDataCommand(bool saveChanges) => SaveChanges = saveChanges;

        public bool SaveChanges { get; }
    }
}
