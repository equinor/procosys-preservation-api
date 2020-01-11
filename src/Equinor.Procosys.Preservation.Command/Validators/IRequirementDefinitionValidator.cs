namespace Equinor.Procosys.Preservation.Command.Validators
{
    public interface IRequirementDefinitionValidator
    {
        bool Exists(int requirementDefinitionId);
    }
}
