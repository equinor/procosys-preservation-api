﻿using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class PagingDtoValidator : AbstractValidator<PagingDto>
    {
        public PagingDtoValidator()
        {
            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Size)
                .GreaterThan(0);
        }
    }
}
