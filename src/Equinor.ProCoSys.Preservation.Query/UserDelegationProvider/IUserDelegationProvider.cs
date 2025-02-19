using Azure.Storage.Blobs.Models;

namespace Equinor.ProCoSys.Preservation.Query.UserDelegationProvider;

public interface IUserDelegationProvider
{
    public UserDelegationKey GetUserDelegationKey();
}
