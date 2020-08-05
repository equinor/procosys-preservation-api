using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter
{
    public class CreateSavedFilterCommandValidator : AbstractValidator<CreateSavedFilterCommand>
    {
        public CreateSavedFilterCommandValidator(ISavedFilterValidator savedFilterValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotExistsASavedFilterWithSameTitleForPerson(command.Title, token))
                .WithMessage(command => $"A saved filter with this title already exists! Title={command.Title}");

            async Task<bool> NotExistsASavedFilterWithSameTitleForPerson(string title, CancellationToken token)
                => !await savedFilterValidator.ExistsWithSameTitleForPersonAsync(title, token);
        }
    }
}
