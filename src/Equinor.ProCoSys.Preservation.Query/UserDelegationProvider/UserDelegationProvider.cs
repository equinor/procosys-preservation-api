using System;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Equinor.ProCoSys.BlobStorage;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.Query.UserDelegationProvider;

public class UserDelegationProvider : IUserDelegationProvider
{
    private readonly BlobServiceClient _blobServiceClient;
    private UserDelegationKey? _userDelegationKeyCached;
    private DateTimeOffset? _expirationTime;

    public UserDelegationProvider(IOptionsMonitor<BlobStorageOptions> options, TokenCredential credential)
    {
        if (string.IsNullOrEmpty(options.CurrentValue.AccountUrl))
        {
            throw new ArgumentNullException(nameof(options.CurrentValue.AccountUrl));
        }

        _blobServiceClient = new BlobServiceClient(new Uri($"{options.CurrentValue.AccountUrl}"), credential);
    }

    public UserDelegationKey GetUserDelegationKey()
    {
        // We get and cache a new User Delegation Key if we don't already have one, or if the one we're
        // using is about to run out in the next hour.
        // We want a bit of leeway here, as we don't want the key to be valid as we check it here as its about to
        // expire, and then have it expire and be invalid as we are trying to use it.
        if (_userDelegationKeyCached == null || _expirationTime <= DateTimeOffset.UtcNow.AddHours(1))
        {
            // Set to about 7 days in the future, because that is the maximum lifetime
            // of a user delegation key.
            var startsOn = DateTimeOffset.UtcNow.AddMinutes(-4);
            var expiresOn = DateTimeOffset.UtcNow.AddDays(7).AddMinutes(-5);
            _userDelegationKeyCached = _blobServiceClient.GetUserDelegationKey(startsOn, expiresOn);
            _expirationTime = _userDelegationKeyCached.SignedExpiresOn;
        }

        return _userDelegationKeyCached;
    }
}
