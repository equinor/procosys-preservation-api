using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.AutoTransfer
{
    public class AutoTransferCommand : IRequest<Result<Unit>>
    {
        public AutoTransferCommand(string projectName, string certificateNo, string certificateType)
        {
            ProjectName = projectName;
            CertificateNo = certificateNo;
            CertificateType = certificateType;
        }

        public string ProjectName { get; }
        public string CertificateNo { get; }
        public string CertificateType { get; }
    }
}
