using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
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
        private readonly IBlobStorage _blobStorage;
        private readonly IBlobPathProvider _blobPathProvider;

        public UploadTagAttachmentCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IBlobStorage blobStorage,
            IBlobPathProvider blobPathProvider)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _blobStorage = blobStorage;
            _blobPathProvider = blobPathProvider;
        }

        public async Task<Result<int>> Handle(UploadTagAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            var attachment = tag.GetAttachmentByFileName(request.FileName);

            if (!request.OverwriteIfExists && attachment != null)
            {
                throw new Exception($"Tag {tag.Id} already have attachment with filename {request.FileName}");
            }

            if (attachment == null)
            {
                attachment = new TagAttachment(
                    _plantProvider.Plant,
                    request.FileName,
                    Guid.NewGuid(), 
                    request.Title);
                tag.AddAttachment(attachment);
            }
            else
            {
                attachment.SetTitle(request.Title, request.FileName);
            }

            var path = _blobPathProvider.CreatePathForAttachment(attachment);

            await _blobStorage.UploadAsync(path, request.Content, request.OverwriteIfExists, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(attachment.Id);
        }
    }
}
