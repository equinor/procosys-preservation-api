using System;

namespace Equinor.Procosys.Preservation.Domain.ProjectAccess
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PathToProjectAttribute : Attribute
    {
        public PathToProjectAttribute(PathToProjectType pathToProjectType, string propertyName)
        {
            PathToProjectType = pathToProjectType;
            PropertyName = propertyName;
        }

        public PathToProjectType PathToProjectType { get; }
        public string PropertyName { get; }
    }
}
