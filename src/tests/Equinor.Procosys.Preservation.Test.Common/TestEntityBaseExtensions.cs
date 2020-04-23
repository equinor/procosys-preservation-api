using System;
using System.Reflection;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Test.Common.ExtentionMethods
{
    public static class TestEntityBaseExtensions
    {
        public static void SetProtectedRowVersionForTesting(this EntityBase entityBase, ulong rowVersion)
        {
            var objType = typeof(EntityBase);
            var property = objType.GetProperty("RowVersion", BindingFlags.Public | BindingFlags.Instance);
            var rowVersionBytes = BitConverter.GetBytes(rowVersion);
            property.SetValue(entityBase, rowVersionBytes);
        }

        public static unsafe byte* GetRowVersionPointer(this EntityBase entityBase)
        {
            fixed (byte* first = &entityBase.RowVersion[7])
            {
                return first;
            }
        }
    }
}
