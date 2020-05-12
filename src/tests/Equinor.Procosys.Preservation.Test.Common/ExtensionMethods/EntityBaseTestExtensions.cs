using System.Reflection;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Test.Common.ExtensionMethods
{
    public static class EntityBaseTestExtensions
    {
        public static void SetProtectedIdForTesting(this EntityBase entityBase, int id)
        {
            var objType = typeof(EntityBase);
            var property = objType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            property.SetValue(entityBase, id);
        }
    }
}
