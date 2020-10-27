namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    // Authenticated client with necessary roles to Create and Update in Library
    public static class AdminClient
    {
        public static TestTokens Tokens
            => new TestTokens
            {
                Oid = "00000000-0000-0000-0000-000000000001",
                FullName = "Arne Admin"
            };
    }
}
