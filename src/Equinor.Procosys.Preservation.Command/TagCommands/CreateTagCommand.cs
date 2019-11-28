using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommand : IRequest<int>
    {
        public string TagNo { get; private set; }
        public string ProjectNo { get; private set; }
        public string Schema { get; private set; }

        public CreateTagCommand(string tagNo, string projectNo, string schema)
        {
            TagNo = tagNo;
            ProjectNo = projectNo;
            Schema = schema;
        }
    }
}
