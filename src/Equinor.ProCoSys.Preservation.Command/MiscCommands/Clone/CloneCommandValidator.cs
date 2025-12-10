using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Caches;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.MiscCommands.Clone
{
    public class CloneCommandValidator : AbstractValidator<CloneCommand>
    {
        private static string[] BasisPlants = { "PCS$STATOIL_BASIS", "PCS$SUN_BASIS", "PCS$WIND_BASIS" };

        public CloneCommandValidator(IPermissionCache permissionCache)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.SourcePlant)
                .MustAsync((_, sourcePlant, token) => UserHaveAccessToPlantAsync(sourcePlant.ToUpperInvariant()))
                .WithMessage(command => $"Source plant is not valid or access missing! Plant={command.SourcePlant}");

            RuleFor(command => command.TargetPlant)
                .MustAsync((_, targetPlant, token) => UserHaveAccessToPlantAsync(targetPlant.ToUpperInvariant()))
                .WithMessage(command => $"Target plant is not valid or access missing! Plant={command.TargetPlant}")
                .Must((_, targetPlant, token) => NotBeABasisPlant(targetPlant.ToUpperInvariant()))
                .WithMessage(command => $"Target plant can not be a basis plant! Plant={command.TargetPlant}");

            async Task<bool> UserHaveAccessToPlantAsync(string plantId)
                => await permissionCache.HasCurrentUserAccessToPlantAsync(plantId, CancellationToken.None);

            bool NotBeABasisPlant(string plantId) => !BasisPlants.Contains(plantId);
        }
    }
}
