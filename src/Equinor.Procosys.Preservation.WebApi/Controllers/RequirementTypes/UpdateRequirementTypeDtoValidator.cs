using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateRequirementTypeDtoValidator : AbstractValidator<UpdateRequirementTypeDto>
    {
        public UpdateRequirementTypeDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(RequirementType.TitleLengthMax);
            RuleFor(x => x.Code)
                .MaximumLength(RequirementType.CodeLengthMax);
            RuleFor(x => x.Icon).NotNull();
            RuleFor(x => x.SortKey).NotNull();
        }
    }
}
