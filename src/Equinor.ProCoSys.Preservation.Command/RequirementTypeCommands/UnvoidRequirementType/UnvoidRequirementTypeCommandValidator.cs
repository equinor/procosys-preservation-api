﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType
{
    public class UnvoidRequirementTypeCommandValidator : AbstractValidator<UnvoidRequirementTypeCommand>
    {
        public UnvoidRequirementTypeCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRowVersionValidator rowVersionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! Requirement type={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAVoidedRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type is not voided! Requirement type={command.RequirementTypeId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token);
            async Task<bool> BeAVoidedRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.IsVoidedAsync(requirementTypeId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
