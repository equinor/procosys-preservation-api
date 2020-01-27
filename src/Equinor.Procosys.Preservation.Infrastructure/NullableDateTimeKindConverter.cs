using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class NullableDateTimeKindConverter : ValueConverter<DateTime?, DateTime?>
    {
        public NullableDateTimeKindConverter() : base(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
        {}
    }
}
