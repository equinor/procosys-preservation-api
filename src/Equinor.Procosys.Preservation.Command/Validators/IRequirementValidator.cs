namespace Equinor.Procosys.Preservation.Command.Validators
{
    public interface IRequirementValidator
    {
        bool Exists(int requirementDefinitionId);
    }
}
