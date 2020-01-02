using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation;
using FluentValidation.Validators;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class StepExistsValidator : AsyncValidatorBase
    {
        private readonly IJourneyRepository _journeyRepository;

        public StepExistsValidator(IJourneyRepository journeyRepository)
            : base("Step with ID {StepId} not found") => _journeyRepository = journeyRepository;

        public override bool ShouldValidateAsync(ValidationContext context) => true;

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            if (context.PropertyValue is int stepId)
            {
                var journey = await _journeyRepository.GetByStepId(stepId);
                if (journey != null)
                {
                    return true;
                }
                context.MessageFormatter.AppendArgument("StepId", stepId);
                return false;
            }
            return false;
        }
    }
}
