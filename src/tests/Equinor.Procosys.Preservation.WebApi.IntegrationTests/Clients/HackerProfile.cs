namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    // Authenticated client without any roles
    public static class HackerProfile
    {
        public static Profile Tokens
            => new Profile
            {
                Oid = "00000000-0000-0000-0000-000000000999",
                FullName = "Harry Hacker"
            };
    }
}
