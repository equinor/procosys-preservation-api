﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommandValidator : AbstractValidator<TransferCommand>
    {
        public TransferCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;
                        
            RuleFor(command => command.Tags)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!")
                .MustAsync(BeInSameProjectAsync)
                .WithMessage("Tags must be in same project!")
                .MustAsync(NotBeAClosedProjectForTagAsync)
                .WithMessage("Project is closed!");

            When(command => command.Tags.Any() && BeUniqueTags(command.Tags), () =>
            {
                RuleForEach(command => command.Tags)
                    .MustAsync((_, tag, __, token) => BeAnExistingTagAsync(tag.Id, token))
                    .WithMessage((_, id) => $"Tag doesn't exist! Tag={id}")
                    .MustAsync((_, tag, __, token) => NotBeAVoidedTagAsync(tag.Id, token))
                    .WithMessage((_, id) => $"Tag is voided! Tag={id}")
                    .MustAsync((_, tag, __, token) => IsReadyToBeTransferredAsync(tag.Id, token))
                    .WithMessage((_, id) => $"Tag can not be transferred! Tag={id}")
                    .Must(tag => HaveAValidRowVersion(tag.RowVersion))
                    .WithMessage((_, tag) => $"Not a valid row version! Row version={tag.RowVersion}");
            });

            bool BeUniqueTags(IEnumerable<IdAndRowVersion> tags)
            {
                var ids = tags.Select(x => x.Id).ToList();
                return ids.Distinct().Count() == ids.Count;
            }
                        
            async Task<bool> BeInSameProjectAsync(IEnumerable<IdAndRowVersion> tags, CancellationToken token)
                => await projectValidator.AllTagsInSameProjectAsync(tags.Select(t => t.Id), token);
            
            async Task<bool> NotBeAClosedProjectForTagAsync(IEnumerable<IdAndRowVersion> tags, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tags.First().Id, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => ! await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> IsReadyToBeTransferredAsync(int tagId, CancellationToken token)
                => await tagValidator.IsReadyToBeTransferredAsync(tagId, token);

            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
