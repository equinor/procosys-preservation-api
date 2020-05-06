using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommandHandler : IRequestHandler<UploadTagAttachmentCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public UploadTagAttachmentCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IBlobStorage blobStorage,
            IOptionsMonitor<AttachmentOptions> attachmentOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _blobStorage = blobStorage;
            _attachmentOptions = attachmentOptions;
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
                attachment = new TagAttachment(
                    _plantProvider.Plant,
                    request.File.FileName,
                    Guid.NewGuid(), 
                    request.Title);
                tag.AddAttachment(attachment);
            }
            else
            {
                attachment.SetTitle(request.Title, request.File.FileName);
            }

            var path = GetPath(nameof(Tag), attachment);

            await _blobStorage.UploadAsync(path, request.File.OpenReadStream(), request.OverwriteIfExists, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(attachment.Id);
        }

        private string GetPath(string folderName, Attachment attachment)
            => $"{_attachmentOptions.CurrentValue.BlobContainer}/{folderName}/{attachment.BlobStorageId.ToString()}/{attachment.FileName}";
    }
}
