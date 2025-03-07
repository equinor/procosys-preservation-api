﻿using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class RequirementValuesDtoValidator : AbstractValidator<RequirementValuesDto>
    {
        public RequirementValuesDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.Comment)
                .MaximumLength(PreservationPeriod.CommentLengthMax);

            RuleForEach(x => x.NumberValues)
                .Must(NumberMustBeEitherNumberOrNa)
                .WithMessage("A number can't both be a real value and NA at same time");
        }

        private bool NumberMustBeEitherNumberOrNa(NumberFieldValueDto nv)
            => !(nv.IsNA && nv.Value.HasValue);
    }
}
