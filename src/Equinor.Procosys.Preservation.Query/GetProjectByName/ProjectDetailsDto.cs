namespace Equinor.Procosys.Preservation.Query.GetProjectByName
{
    public class ProjectDetailsDto
    {
        public ProjectDetailsDto(int id, string name, string description, bool isClosed)
        {
            Id = id;
            Name = name;
            Description = description;
            IsClosed = isClosed;
        }

        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsClosed { get; }
    }
}
