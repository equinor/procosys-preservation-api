using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class RequirementValuesDtoValidator : AbstractValidator<RequirementValuesDto>
    {
        public RequirementValuesDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.Comment)
                .MaximumLength(PreservationPeriod.CommentLengthMax);
        }
    }
}
