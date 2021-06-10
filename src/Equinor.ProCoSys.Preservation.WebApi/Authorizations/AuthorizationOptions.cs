using System;
using System.Collections.Generic;
using System.Linq;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class AuthorizationOptions
    {
        private readonly List<Guid> _userOids = new List<Guid>();
        private string _crossPlantUserOidList;

        public List<Guid> CrossPlantUserOids() => _userOids;

        public override string ToString()
            => string.Join(",", _userOids.Select(oid => oid.ToString("B")));

        public string CrossPlantUserOidList
        {
            get => _crossPlantUserOidList;
            set
            {
                _crossPlantUserOidList = value;
                Transform();
            }
        }

        private void Transform()
        {
            if (string.IsNullOrEmpty(_crossPlantUserOidList))
            {
                return;
            }
            foreach (var oid in _crossPlantUserOidList.Split(",", StringSplitOptions.TrimEntries))
            {
                if (Guid.TryParse(oid, out var guid))
                {
                    _userOids.Add(guid);
                }
            }
        }
    }
}
