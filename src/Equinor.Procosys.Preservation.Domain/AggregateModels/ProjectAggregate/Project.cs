using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Project : SchemaEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
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
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void AddTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.Schema != Schema)
            {
                throw new ArgumentException($"Can't relate item in {tag.Schema} to item in {Schema}");
            }

            _tags.Add(tag);
        }

        public void Close() => IsClosed = true;

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            ModifiedById = modifiedBy.Id;
        }
    }
}
