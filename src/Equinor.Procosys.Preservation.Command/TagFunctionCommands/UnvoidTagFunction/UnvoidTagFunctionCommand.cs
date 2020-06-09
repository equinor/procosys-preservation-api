using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.UnvoidTagFunction
{
    public class UnvoidTagFunctionCommand : IRequest<Result<string>>
    {
        public UnvoidTagFunctionCommand(string tagFunctionCode, string registerCode, string rowVersion, Guid currentUserOid)
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
