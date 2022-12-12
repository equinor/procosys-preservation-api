namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class TagFunctionTmpTopic
    {
        public const string TopicName = "tagfunction";

        public string Plant
        {
            get;
            set;
        }

        public string ProCoSysGuid
        {
            get;
            set;
        }

        public string Behavior
        {
            get;
            set;
        }

        public string Code
        {
            get;
            set;
        }

        public string CodeOld
        {
            get;
            set;
        }

        public string RegisterCode
        {
            get;
            set;
        }

        public string RegisterCodeOld
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool IsVoided
        {
            get;
            set;
        }
    }
}
