using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;
using FluentValidation.Validators;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        private readonly IJourneyRepository _journeyRepository;

        public CreateTagCommandValidator(IJourneyRepository journeyRepository)
        {
            _journeyRepository = journeyRepository;

            RuleFor(x => x.JourneyId)
                .JourneyMustExist(_journeyRepository);
        }
    }

    public class JourneyExistsValidator : AsyncValidatorBase
    {
        private readonly IJourneyRepository _journeyRepository;

        public JourneyExistsValidator(IJourneyRepository journeyRepository)
            : base("{PropertyName} with ID {JourneyId} not found") => _journeyRepository = journeyRepository;

        public override bool ShouldValidateAsync(ValidationContext context) => true;

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            if (context.PropertyValue is int journeyId)
            {
                var journey = await _journeyRepository.GetByIdAsync(journeyId);
                if (journey != null)
                {
                    return true;
                }
                context.MessageFormatter.AppendArgument("JourneyId", journeyId);
                return false;
            }
            return false;
        }
    }

    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, int> JourneyMustExist<T>(this IRuleBuilder<T, int> ruleBuilder, IJourneyRepository journeyRepository) =>
            ruleBuilder
                .SetValidator(new JourneyExistsValidator(journeyRepository));
    }
}
