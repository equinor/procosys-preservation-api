using System;

namespace Equinor.ProCoSys.Preservation.Domain.Time
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
