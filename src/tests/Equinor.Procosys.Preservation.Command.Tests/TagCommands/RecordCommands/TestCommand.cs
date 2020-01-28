using Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordCommands
{
    public class TestCommand : RecordCommand
    {
        public TestCommand(int tagId, int fieldId) : base(tagId, fieldId)
        {
        }
    }
}
