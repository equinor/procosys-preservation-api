using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using FluentValidation;
using FluentValidation.Validators;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class ResponsibleExistsValidator : AsyncValidatorBase
    {
        private readonly IResponsibleRepository _responsibleRepository;

        public ResponsibleExistsValidator(IResponsibleRepository responsibleRepository)
            : base("{PropertyName} with ID {ResponsibleId} not found") => _responsibleRepository = responsibleRepository;

        public override bool ShouldValidateAsync(ValidationContext context) => true;

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            if (context.PropertyValue is int responsibleId)
            {
                var responsible = await _responsibleRepository.GetByIdAsync(responsibleId);
                if (responsible != null)
                {
                    return true;
                }
                context.MessageFormatter.AppendArgument("ResponsibleId", responsibleId);
                return false;
            }
            return false;
        }
    }
}
