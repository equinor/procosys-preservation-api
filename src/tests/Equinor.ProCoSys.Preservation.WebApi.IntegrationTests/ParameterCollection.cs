using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    public class ParameterCollection : NameValueCollection
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var key in AllKeys)
            {
                var values = GetValues(key);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        sb.Append(sb.Length > 0 ? "&" : "?");
                        sb.AppendFormat("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value));
                    }
                }
            }

            return sb.ToString();
        }

        public override void Add(string name, string value)
            => base.Add(name, value ?? "");
    }
}
