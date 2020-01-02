using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using FluentValidation;
using FluentValidation.Validators;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class ModeExistsValidator : AsyncValidatorBase
    {
        private readonly IModeRepository _modeRepository;

        public ModeExistsValidator(IModeRepository modeRepository)
            : base("{PropertyName} with ID {ModeId} not found") => _modeRepository = modeRepository;

        public override bool ShouldValidateAsync(ValidationContext context) => true;

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            if (context.PropertyValue is int modeId)
            {
                var mode = await _modeRepository.GetByIdAsync(modeId);
                if (mode != null)
                {
                    return true;
                }
                context.MessageFormatter.AppendArgument("ModeId", modeId);
                return false;
            }
            return false;
        }
    }
}
