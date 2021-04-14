namespace Equinor.ProCoSys.Preservation.Query
{
    public static class Strings
    {
        public static string EntityNotFound(string entity, int id) => $"{entity} with ID {id} not found";
        public static string EntityNotFound(string entity, string name) => $"{entity} with Name {name} not found";
    }
}
