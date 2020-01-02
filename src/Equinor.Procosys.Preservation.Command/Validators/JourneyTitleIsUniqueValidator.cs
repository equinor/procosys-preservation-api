using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;
using FluentValidation.Validators;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class JourneyTitleIsUniqueValidator : AsyncValidatorBase
    {
        private readonly IJourneyRepository _journeyRepository;

        public JourneyTitleIsUniqueValidator(IJourneyRepository journeyRepository)
            : base("{PropertyName} must be unique") => _journeyRepository = journeyRepository;

        public override bool ShouldValidateAsync(ValidationContext context) => true;

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            if (context.PropertyValue is string journeyTitle)
            {
                var journey = await _journeyRepository.GetByTitleAsync(journeyTitle);
                if (journey == null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
