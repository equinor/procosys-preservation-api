namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    // Authenticated client with necessary roles to Update tags as a Preserver
    public static class PreserverClient
    {
        public static TestTokens Tokens
            => new TestTokens
            {
                Oid = "00000000-0000-0000-0000-000000000003",
                FullName = "Peder Preserver"
            };
    }
}
