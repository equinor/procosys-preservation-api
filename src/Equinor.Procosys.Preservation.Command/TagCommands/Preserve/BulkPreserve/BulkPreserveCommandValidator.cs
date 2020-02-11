using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve.BulkPreserve
{
    public class BulkPreserveCommandValidator : AbstractValidator<PreserveCommand>
    {
        public BulkPreserveCommandValidator(
            PreserveCommandValidator preserveCommandValidator,
            ITagValidator tagValidator,
            ITimeService timeService)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(tag => tag)
                .SetValidator(preserveCommandValidator);

            When(tag => tag.TagIds.Any() && BeUniqueTags(tag.TagIds), () =>
            {
                RuleForEach(s => s.TagIds)
                    .Must(BeReadyToBeBulkPreserved)
                    .WithMessage((x, id) => $"Tag is not ready to be bulk preserved! Tag={id}");
            });

            bool BeUniqueTags(IEnumerable<int> tagIds)
            {
                var ids = tagIds.ToList();
                return ids.Distinct().Count() == ids.Count;
            }
            
            bool BeReadyToBeBulkPreserved(int tagId) => tagValidator.ReadyToBeBulkPreserved(tagId, timeService.GetCurrentTimeUtc());
        }
    }
}
