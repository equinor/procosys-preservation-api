using System;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class ActionDto
    {
        public ActionDto(
            int id, 
            string description,
            DateTime? dueTimeUtc)
        {
            Id = id;
            Description = description;
            DueTimeUtc = dueTimeUtc;
        }

        public int Id { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
    }
}
