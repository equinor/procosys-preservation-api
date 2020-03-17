using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Step : SchemaEntityBase, ICreationAuditable, IModificationAuditable
    {
        public const int TitleLengthMin = 1;
        public const int TitleLengthMax = 64;
        protected Step()
            : base(null)
        {
        }

        public Step(string schema, string title, Mode mode, Responsible responsible)
            : base(schema)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (mode == null)
            {
                throw new ArgumentNullException(nameof(mode));
            }

            if (responsible == null)
            {
                throw new ArgumentNullException(nameof(responsible));
            }

            if (mode.Schema != schema)
            {
                throw new ArgumentException($"Can't relate item in {mode.Schema} to item in {schema}");
            }

            if (responsible.Schema != schema)
            {
                throw new ArgumentException($"Can't relate item in {responsible.Schema} to item in {schema}");
            }

            Title = title;
            SortKey = 0;
            ModeId = mode.Id;
            ResponsibleId = responsible.Id;
        }

        public string Title { get; private set; }
        public int ModeId { get; private set; }
        public int ResponsibleId { get; private set; }
        public int SortKey { get; set; }  // sortKey will be set correct in later PBI when impl UI for Add, MoveUp and MoveDown of Steps in Journey
        public bool IsVoided { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

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
