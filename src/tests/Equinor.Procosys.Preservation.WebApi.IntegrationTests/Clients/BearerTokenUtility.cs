using System;
using System.Text;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    public static class BearerTokenUtility
    {
        /// <summary>
        /// Wraps <typeparamref name="T"/> by serializing, encoding and then converting to base 64 string.
        /// "Bearer" is also added, making it ready to be added as Authorization header
        /// </summary>
        /// <typeparam name="T">The type of the class that is serialized</typeparam>
        /// <param name="tokenClass">The instance of the token to be wrapped</param>
        /// <returns>Serialized, encoded string ready for authorization header</returns>
        public static string WrapAuthToken<T>(T tokenClass) where T : TestProfile
        {
            var serialized = JsonConvert.SerializeObject(tokenClass);
            var tokenBytes = Encoding.UTF8.GetBytes(serialized);
            var tokenString = Convert.ToBase64String(tokenBytes);

            return $"Bearer {tokenString}";
        }
    }
}
