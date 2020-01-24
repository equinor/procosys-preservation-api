using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class RequirementDto
    {
        private readonly DateTime _currentDateTimeUtc;

        public RequirementDto(DateTime currentDateTimeUtc, DateTime? nextDueTimeUtc)
        {
            if (nextDueTimeUtc.HasValue && nextDueTimeUtc.Value.Kind != currentDateTimeUtc.Kind)
            {
                throw new ArgumentException($"{nameof(nextDueTimeUtc)} and {nameof(currentDateTimeUtc)} has different kinds");
            }

            NextDueTimeUtc = nextDueTimeUtc;
            _currentDateTimeUtc = currentDateTimeUtc;
        }

        public DateTime? NextDueTimeUtc { get; }

        public string NextDueAsYearAndWeek => NextDueTimeUtc?.FormatAsYearAndWeekString();

        public int? NextDueWeeks
        {
            get
            {
                if (!NextDueTimeUtc.HasValue)
                {
                    return null;
                }

                return _currentDateTimeUtc.GetWeeksUntilDate(NextDueTimeUtc.Value);
            }
        }
    }
}
