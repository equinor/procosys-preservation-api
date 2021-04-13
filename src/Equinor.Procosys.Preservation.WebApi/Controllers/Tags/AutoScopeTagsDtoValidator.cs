using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class AutoScopeTagsDtoValidator : AbstractValidator<AutoScopeTagsDto>
    {
        public AutoScopeTagsDtoValidator()
        {
            RuleFor(x => x).NotNull();
            
            RuleFor(x => x.ProjectName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Project.NameLengthMax);

            RuleFor(x => x.TagNos)
                .NotNull();

            RuleForEach(x => x.TagNos)
                .NotEmpty()
                .MaximumLength(Tag.TagNoLengthMax);

            RuleFor(x => x.Remark)
                .MaximumLength(Tag.RemarkLengthMax);

            RuleFor(x => x.StorageArea)
                .MaximumLength(Tag.StorageAreaLengthMax);
        }
    }
}
