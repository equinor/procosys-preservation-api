using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter
{
    public class CreateSavedFilterCommandValidator : AbstractValidator<CreateSavedFilterCommand>
    {
        public CreateSavedFilterCommandValidator(
            ISavedFilterValidator savedFilterValidator,
            IProjectValidator projectValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotExistsASavedFilterWithSameTitleForPerson(command.Title, token))
                .WithMessage(command => $"A saved filter with this title already exists! Title={command.Title}");
            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingProject(command.ProjectName, token))
                .WithMessage(command => $"Project doesn't exist! Project={command.ProjectName}");
             

            async Task<bool> NotExistsASavedFilterWithSameTitleForPerson(string title, CancellationToken token)
                => !await savedFilterValidator.ExistsWithSameTitleForPersonAsync(title, token);
            async Task<bool> BeAnExistingProject(string projectName, CancellationToken token)
                => await projectValidator.ExistsAsync(projectName, token);
        }
    }
}
