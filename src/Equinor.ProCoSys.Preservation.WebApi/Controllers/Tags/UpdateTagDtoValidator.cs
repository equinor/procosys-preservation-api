using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagDtoValidator : AbstractValidator<UpdateTagDto>
    {
        public UpdateTagDtoValidator()
        {
            RuleFor(x => x).NotNull();

            RuleFor(x => x.Remark)
                .MaximumLength(Tag.RemarkLengthMax);
            
            RuleFor(x => x.StorageArea)
                .MaximumLength(Tag.StorageAreaLengthMax);
        }
    }
}
