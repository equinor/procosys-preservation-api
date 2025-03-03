using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Project
{
    public class ProjectApiForUserService(
        IMainApiClientForUser mainApiClient,
        IOptionsSnapshot<MainApiOptions> options)
        : ProjectApiService(mainApiClient, options), IProjectApiForUserService;
}
