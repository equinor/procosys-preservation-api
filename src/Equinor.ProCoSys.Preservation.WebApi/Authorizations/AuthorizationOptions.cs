using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class AuthorizationOptions
    {
        public List<Guid> CrossPlantUserOids()
        {
            var userOids = new List<Guid>();
            if (CrossPlantUserOidList == null)
            {
                return userOids;
            }

            foreach (var oid in CrossPlantUserOidList.Split(","))
            {
                if (Guid.TryParse(oid, out var guid))
                {
                    userOids.Add(guid);
                }
            }

            return userOids;
        }

        public string CrossPlantUserOidList { get; set; }
    }
}
