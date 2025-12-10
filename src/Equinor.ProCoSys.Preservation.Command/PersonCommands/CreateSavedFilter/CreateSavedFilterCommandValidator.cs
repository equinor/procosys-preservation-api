using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.SavedFilterValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.PersonCommands.CreateSavedFilter
{
    public class CreateSavedFilterCommandValidator : AbstractValidator<CreateSavedFilterCommand>
    {
        public CreateSavedFilterCommandValidator(
            ISavedFilterValidator savedFilterValidator,
            IProjectValidator projectValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotExistsASavedFilterWithSameTitleForPerson(command.Title, command.ProjectName, token))
                .WithMessage(command => $"A saved filter with this title already exists! Title={command.Title}");
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingProject(command.ProjectName, token))
                .WithMessage(command => $"Project doesn't exist! Project={command.ProjectName}");

            async Task<bool> NotExistsASavedFilterWithSameTitleForPerson(string title, string projectName, CancellationToken token)
                => !await savedFilterValidator.ExistsWithSameTitleForPersonInProjectAsync(title, projectName, token);
            async Task<bool> BeAnExistingProject(string projectName, CancellationToken token)
                => await projectValidator.ExistsAsync(projectName, token);
        }
    }
}
