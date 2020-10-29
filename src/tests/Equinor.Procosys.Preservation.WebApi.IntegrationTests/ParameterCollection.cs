using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public class ParameterCollection : Dictionary<string,string>
    {
        public ParameterCollection()
        { 
        }

        public ParameterCollection(ParameterCollection parameters)
        {
            foreach (var kvp in parameters)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in this)
            {
                sb.Append(sb.Length > 0 ? "&" : "?");
                sb.AppendFormat($"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}");
            }
            return sb.ToString();
        }
    }
}
