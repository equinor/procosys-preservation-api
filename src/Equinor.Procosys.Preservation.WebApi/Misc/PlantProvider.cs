using System;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class PlantProvider : IPlantProvider
    {
        public const string PlantHeader = "x-plant";

        private readonly IHttpContextAccessor _accessor;
        private string _temporaryPlant;

        public PlantProvider(IHttpContextAccessor accessor) => _accessor = accessor;

        public string Plant
        {
            get
            {
                if (_temporaryPlant != null)
                {
                    return _temporaryPlant;
                }
                return _accessor?.HttpContext?.Request?.Headers[PlantHeader].ToString().ToUpperInvariant() ??
                       throw new Exception("Could not determine current plant");
            }
        }

        public void SetTemporaryPlant(string plant) => _temporaryPlant = plant;

        public void ReleaseTemporaryPlant() => _temporaryPlant = null;
    }
}
