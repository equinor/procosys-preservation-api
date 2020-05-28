namespace Equinor.Procosys.Preservation.WebApi
{
    public class Permissions
    {
        public const string TAG_READ = "TAG/READ";
        
        public const string LIBRARY_GENERAL_READ = "LIBRARY_GENERAL/READ";

        public const string LIBRARY_PRESERVATION_READ = "LIBRARY_PRESERVATION/READ";
        public const string LIBRARY_PRESERVATION_CREATE = "LIBRARY_PRESERVATION/CREATE";
        public const string LIBRARY_PRESERVATION_WRITE = "LIBRARY_PRESERVATION/WRITE";
        public const string LIBRARY_PRESERVATION_DELETE = "LIBRARY_PRESERVATION/DELETE";
        public const string LIBRARY_PRESERVATION_VOIDUNVOID = "LIBRARY_PRESERVATION/VOID/UNVOID";

        public const string PRESERVATION_PLAN_CREATE = "PRESERVATION_PLAN/CREATE";
        public const string PRESERVATION_PLAN_WRITE = "PRESERVATION_PLAN/WRITE";
        public const string PRESERVATION_PLAN_DELETE = "PRESERVATION_PLAN/DELETE";
        public const string PRESERVATION_PLAN_VOIDUNVOID = "PRESERVATION_PLAN/VOID/UNVOID";

        public const string PRESERVATION_ATTACHFILE = "PRESERVATION/ATTACHFILE";
        public const string PRESERVATION_DETACHFILE = "PRESERVATION/DETACHFILE";
        public const string PRESERVATION_READ = "PRESERVATION/READ";
        public const string PRESERVATION_CREATE = "PRESERVATION/CREATE";
        public const string PRESERVATION_WRITE = "PRESERVATION/WRITE";
        public const string PRESERVATION_DELETE = "PRESERVATION/DELETE";
    }
}
