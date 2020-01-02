using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using FluentValidation;
using FluentValidation.Validators;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class TagExistsValidator : AsyncValidatorBase
    {
        private readonly ITagRepository _tagRepository;

        public TagExistsValidator(ITagRepository tagRepository)
            : base("{PropertyName} with ID {TagId} not found") => _tagRepository = tagRepository;

        public override bool ShouldValidateAsync(ValidationContext context) => true;

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            if (context.PropertyValue is int tagId)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId);
                if (tag != null)
                {
                    return true;
                }
                context.MessageFormatter.AppendArgument("TagId", tagId);
                return false;
            }
            return false;
        }
    }
}
