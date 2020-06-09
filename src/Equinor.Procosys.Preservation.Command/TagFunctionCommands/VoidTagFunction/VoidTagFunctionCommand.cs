using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.VoidTagFunction
{
    public class VoidTagFunctionCommand : IRequest<Result<string>>
    {
        public VoidTagFunctionCommand(string tagFunctionCode, string registerCode, string rowVersion, Guid currentUserOid)
        {
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
