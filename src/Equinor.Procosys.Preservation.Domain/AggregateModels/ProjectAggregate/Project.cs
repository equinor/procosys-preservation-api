using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Project : SchemaEntityBase, IAggregateRoot
    {
        private readonly List<Tag> _tags = new List<Tag>();
        
        public const int NameLengthMax = 30;
        public const int DescriptionLengthMax = 1000;

        protected Project()
            : base(null)
        {
        }

        public Project(string schema, string name, string description)
            : base(schema)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsClosed { get; private set; }
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        public void AddTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.Schema != Schema)
            {
                throw new ArgumentException($"Can't add item in {tag.Schema} to {Schema}");
            }

            _tags.Add(tag);
        }

        public void Close() => IsClosed = true;
    }
}
