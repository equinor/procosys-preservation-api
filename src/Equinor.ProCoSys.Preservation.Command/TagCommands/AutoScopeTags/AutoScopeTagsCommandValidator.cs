﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.AutoScopeTags
{
    public class AutoScopeTagsCommandValidator : AbstractValidator<AutoScopeTagsCommand>
    {
        public AutoScopeTagsCommandValidator(
            ITagValidator tagValidator,
            IStepValidator stepValidator,
            IProjectValidator projectValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.TagNos)
                .Must(r => r.Any())
                .WithMessage("At least 1 tag must be given!")
                .Must(BeUniqueTagNos)
                .WithMessage("Tags must be unique!");

            RuleForEach(command => command.TagNos)
                .MustAsync((command, tagNo, _, token) => NotBeAnExistingTagWithinProjectAsync(tagNo, command.ProjectName, token))
                .WithMessage((_, tagNo) => $"Tag already exists in scope for project! Tag={tagNo}");

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAnExistingAndClosedProjectAsync(command.ProjectName, token))
                .WithMessage(command => $"Project is closed! Project={command.ProjectName}")
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepId, token))
                .WithMessage(command => $"Step doesn't exist! Step={command.StepId}")
                .MustAsync((command, token) => NotBeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is voided! Step={command.StepId}");
                        
            bool BeUniqueTagNos(IEnumerable<string> tagNos)
            {
                var lowerTagNos = tagNos.Select(t => t.ToLower()).ToList();
                return lowerTagNos.Distinct().Count() == lowerTagNos.Count;
            }

            async Task<bool> NotBeAnExistingTagWithinProjectAsync(string tagNo, string projectName, CancellationToken token) =>
                !await tagValidator.ExistsAsync(tagNo, projectName, token);

            async Task<bool> NotBeAnExistingAndClosedProjectAsync(string projectName, CancellationToken token)
                => !await projectValidator.IsExistingAndClosedAsync(projectName, token);

            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);

            async Task<bool> NotBeAVoidedStepAsync(int stepId, CancellationToken token)
                => !await stepValidator.IsVoidedAsync(stepId, token);
        }
    }
}
