using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public class ParameterCollection : Dictionary<string,string>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in this)
            {
                sb.Append(sb.Length > 0 ? "&" : "?");
                sb.AppendFormat("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value));
            }
            return sb.ToString();
        }
    }
}
