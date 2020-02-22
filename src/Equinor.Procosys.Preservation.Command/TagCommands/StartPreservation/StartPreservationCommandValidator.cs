using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Validators.Project;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommandValidator : AbstractValidator<StartPreservationCommand>
    {
        public StartPreservationCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
                        
            RuleFor(command => command.TagIds)
                .Must(ids => ids != null && ids.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTags)
                .WithMessage("Tags must be unique!");

            When(tag => tag.TagIds.Any() && BeUniqueTags(tag.TagIds), () =>
            {
                RuleForEach(command => command.TagIds)
                    .MustAsync((_, tagId, __, token) => NotBeAClosedProjectForTagAsync(tagId, token))
                    .WithMessage((_, id) => $"Project for tag is closed! Tag={id}")
                    .Must(BeAnExistingTag)
                    .WithMessage((_, id) => $"Tag doesn't exists! Tag={id}")
                    .Must(NotBeAVoidedTag)
                    .WithMessage((_, id) => $"Tag is voided! Tag={id}")
                    .Must(PreservationIsNotStarted)
                    .WithMessage((_, id) => $"Tag must have status {PreservationStatus.NotStarted} to start! Tag={id}")
                    .Must(HaveAtLeastOneNonVoidedRequirement)
                    .WithMessage((_, id) => $"Tag do not have any non voided requirement! Tag={id}")
                    .Must(HaveExistingRequirementDefinitions)
                    .WithMessage((_, id) => $"A requirement definition doesn't exists! Tag={id}");
            });

            bool BeUniqueTags(IEnumerable<int> tagIds)
            {
                var ids = tagIds.ToList();
                return ids.Distinct().Count() == ids.Count;
            }
            
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken cancellationToken)
                => !await projectValidator.IsClosedForTagAsync(tagId, cancellationToken);

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool PreservationIsNotStarted(int tagId) => tagValidator.VerifyPreservationStatus(tagId, PreservationStatus.NotStarted);

            bool HaveAtLeastOneNonVoidedRequirement(int tagId) => tagValidator.HasANonVoidedRequirement(tagId);
            
            bool HaveExistingRequirementDefinitions(int tagId) => tagValidator.AllRequirementDefinitionsExist(tagId);
        }
    }
}
