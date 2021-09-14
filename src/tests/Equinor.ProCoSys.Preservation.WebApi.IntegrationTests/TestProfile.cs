using System;
using System.Text;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    public class TestProfile
    {
        public string Oid { get; set; }
        public Guid AzureOid => new Guid(Oid);
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public bool IsAppToken { get; set; } = false;
     
        public override string ToString() => $"{FullName} {Oid}";
        
        /// <summary>
        /// Wraps profile by serializing, encoding and then converting to base 64 string.
        /// "Bearer" is also added, making it ready to be added as Authorization header
        /// </summary>
        /// <returns>Serialized, encoded string ready for authorization header</returns>
        public string CreateBearerToken()
        {
            var serialized = JsonConvert.SerializeObject(this);
            var tokenBytes = Encoding.UTF8.GetBytes(serialized);
            var tokenString = Convert.ToBase64String(tokenBytes);

            return $"Bearer {tokenString}";
        }
    }
}
