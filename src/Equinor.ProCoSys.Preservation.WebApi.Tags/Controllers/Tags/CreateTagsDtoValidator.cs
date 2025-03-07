﻿using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class CreateTagsDtoValidator : AbstractValidator<CreateTagsDto>
    {
        public CreateTagsDtoValidator()
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
            
            RuleFor(x => x.Requirements)
                .Must(BeUniqueRequirements)
                .WithMessage("Requirement definitions must be unique!");

            RuleForEach(x => x.Requirements)
                .Must(RequirementMustHavePositiveInterval)
                .WithMessage("Week interval must be positive");

            RuleFor(x => x.Remark)
                .MaximumLength(Tag.RemarkLengthMax);
            
            RuleFor(x => x.StorageArea)
                .MaximumLength(Tag.StorageAreaLengthMax);

            bool RequirementMustHavePositiveInterval(TagRequirementDto dto) => dto.IntervalWeeks > 0;
                        
            bool BeUniqueRequirements(IEnumerable<TagRequirementDto> requirements)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }
        }
    }
}
