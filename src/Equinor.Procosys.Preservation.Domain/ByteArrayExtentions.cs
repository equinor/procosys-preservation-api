using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public static class ByteArrayExtensions
    {
        public static ulong ToULong(this byte[] bytes) => (ulong)BitConverter.ToInt64(bytes);
    }
}
