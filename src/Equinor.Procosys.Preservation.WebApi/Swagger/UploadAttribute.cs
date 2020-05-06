using System;

namespace Equinor.Procosys.Preservation.WebApi.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UploadAttribute : Attribute
    {
        public UploadAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }
    }
}
