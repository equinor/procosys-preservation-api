using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Step : PlantEntityBase, ICreationAuditable, IModificationAuditable, IVoidable
    {
        public const int TitleLengthMin = 1;
        public const int TitleLengthMax = 64;

        protected Step()
            : base(null)
        {
        }

        public Step(string plant, string title, Mode mode, Responsible responsible)
            : base(plant)
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

            if (mode.Plant != plant)
            {
                throw new ArgumentException($"Can't relate item in {mode.Plant} to item in {plant}");
            }

            if (responsible.Plant != plant)
            {
                throw new ArgumentException($"Can't relate item in {responsible.Plant} to item in {plant}");
            }

            Title = title;
            SortKey = 0;
            ModeId = mode.Id;
            IsSupplierStep = mode.ForSupplier;
            ResponsibleId = responsible.Id;
        }

        public string Title { get; set; }
        public int ModeId { get; private set; }
        public bool IsSupplierStep { get; private set; }

        public AutoTransferMethod AutoTransferMethod { get; set; }

        public int ResponsibleId { get; private set; }

        public int SortKey { get; set; }
        public bool IsVoided { get; set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }
        
        public override string ToString() => $"{Title} ({SortKey})";

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }

            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }

            ModifiedById = modifiedBy.Id;
        }

        public void SetMode(Mode mode)
        {
            if (mode == null)
            {
                throw new ArgumentNullException(nameof(mode));
            }

            IsSupplierStep = mode.ForSupplier;
            ModeId = mode.Id;
        }

        public void SetResponsible(Responsible responsible)
        {
            if (responsible == null)
            {
                throw new ArgumentNullException(nameof(responsible));
            }

            ResponsibleId = responsible.Id;
        }
    }
}
