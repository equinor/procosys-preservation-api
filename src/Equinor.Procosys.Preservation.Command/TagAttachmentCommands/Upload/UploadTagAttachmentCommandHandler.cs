using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommandHandler : IRequestHandler<UploadTagAttachmentCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public UploadTagAttachmentCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork, IPlantProvider plantProvider)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(UploadTagAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            var attachment = tag.GetAttachmentByFileName(request.File.FileName);

            if (!request.OverwriteIfExists && attachment != null)
            {
                throw new Exception($"Tag {tag.Id} already have attachment with filename {request.File.FileName}");
            }

            if (attachment == null)
            {
                var blobStorageId = new Guid("{33CA2707-B306-43BE-95C6-779996ED2F96}"); // todo integrate with blobstorage
                attachment = new TagAttachment(
                 _plantProvider.Plant,
                 request.File.FileName,
                 blobStorageId,
                request.Title);

                tag.AddAttachment(attachment);
            }
            else
            {
                // todo integrate with blobstorage and overwrite file
                attachment.Title = request.Title;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(attachment.Id);
        }
    }
}
