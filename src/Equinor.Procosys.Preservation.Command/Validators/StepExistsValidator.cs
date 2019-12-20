using System.Linq;
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
            : base("Step with ID {StepId} on Journey with ID {JourneyId} not found") => _journeyRepository = journeyRepository;

        public override bool ShouldValidateAsync(ValidationContext context) => true;

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            if (context.PropertyValue is JourneyStep journeyStep)
            {
                var journey = await _journeyRepository.GetByIdAsync(journeyStep.JourneyId);
                if (journey != null)
                {
                    if (journey.Steps.Any(x => x.Id == journeyStep.StepId))
                    {
                        return true;
                    }
                }
                context.MessageFormatter.AppendArgument("StepId", journeyStep.StepId);
                context.MessageFormatter.AppendArgument("JourneyId", journeyStep.JourneyId);
                return false;
            }
            return false;
        }

        public class JourneyStep
        {
            public JourneyStep(int journeyId, int stepId)
            {
                JourneyId = journeyId;
                StepId = stepId;
            }

            public int JourneyId { get; }
            public int StepId { get; }
        }
    }
}
