using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class RequirementDto
    {
        private readonly DateTime? _nextDueTimeUtc;
        private readonly PreservationDate _nextPreservationDate;

        public RequirementDto(bool needsUserInput, DateTime? nextDueTimeUtc)
        {
            NeedsUserInput = needsUserInput;
            _nextPreservationDate = nextDueTimeUtc.HasValue ? new PreservationDate(nextDueTimeUtc.Value) : null;
            _nextDueTimeUtc = nextDueTimeUtc;
        }

        public bool NeedsUserInput { get; }

        public string NextDueWeek => _nextPreservationDate?.ToString();
    }
}
