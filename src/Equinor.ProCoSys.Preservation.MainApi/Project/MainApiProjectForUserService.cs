using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Project
{
    public class MainApiProjectForUserService(
        IMainApiClientForUser mainApiClient,
        IOptionsSnapshot<MainApiOptions> options)
        : MainApiProjectService(mainApiClient, options), IMainApiProjectApiForUserService;
}
