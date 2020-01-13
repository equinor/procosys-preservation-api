namespace Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition
{
    public interface IRequirementDefinitionValidator
    {
        bool Exists(int requirementDefinitionId);
    }
}
