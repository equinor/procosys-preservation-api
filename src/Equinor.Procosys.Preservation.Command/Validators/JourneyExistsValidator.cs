using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;
using FluentValidation.Validators;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class JourneyExistsValidator : AsyncValidatorBase
    {
        private readonly IJourneyRepository _journeyRepository;

        public JourneyExistsValidator(IJourneyRepository journeyRepository)
            : base("Journey with ID {JourneyId} not found") => _journeyRepository = journeyRepository;

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
}
