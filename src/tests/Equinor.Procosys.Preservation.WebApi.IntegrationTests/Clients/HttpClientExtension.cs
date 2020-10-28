using System.Net.Http;
using Equinor.Procosys.Preservation.WebApi.Middleware;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    public static class HttpClientExtension
    {
        public static void UpdatePlantInHeader(this HttpClient client, string requestingPlant)
        {
            if (client.DefaultRequestHeaders.Contains(CurrentPlantMiddleware.PlantHeader))
            {
                client.DefaultRequestHeaders.Remove(CurrentPlantMiddleware.PlantHeader);
            }
            client.DefaultRequestHeaders.Add(CurrentPlantMiddleware.PlantHeader, requestingPlant);
        }
    }
}
