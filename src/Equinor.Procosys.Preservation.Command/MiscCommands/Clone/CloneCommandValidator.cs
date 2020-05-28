using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Plant;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.MiscCommands.Clone
{
    public class CloneCommandValidator : AbstractValidator<CloneCommand>
    {
        private static string[] BasisPlants = {"PCS$STATOIL_BASIS", "PCS$SUN_BASIS", "PCS$WIND_BASIS"};

        public CloneCommandValidator(IPlantCache plantCache)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command.SourcePlant)
                .MustAsync((_, sourcePlant, token) => BeAValidPlantAsync(sourcePlant.ToUpperInvariant()))
                .WithMessage(command => $"Source plant is not valid! Plant={command.SourcePlant}");

            RuleFor(command => command.TargetPlant)
                .MustAsync((_, targetPlant, token) => BeAValidPlantAsync(targetPlant.ToUpperInvariant()))
                .WithMessage(command => $"Source plant is not valid! Plant={command.TargetPlant}")
                .Must((_, targetPlant, token) => NotBeABasisPlant(targetPlant.ToUpperInvariant()))
                .WithMessage(command => $"Basis plant can't be a target plant for clone! Plant={command.TargetPlant}");

            async Task<bool> BeAValidPlantAsync(string plantId)
                => await plantCache.IsValidPlantForCurrentUserAsync(plantId);
            
            bool NotBeABasisPlant(string plantId) => !BasisPlants.Contains(plantId);
        }
    }
}
