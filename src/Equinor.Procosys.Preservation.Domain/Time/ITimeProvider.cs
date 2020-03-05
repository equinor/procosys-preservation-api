using System;

namespace Equinor.Procosys.Preservation.Domain.Time
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
