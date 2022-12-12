namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class ProjectTmpTopic
    {
        public const string TopicName = "project";

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

        public string ProjectName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool IsClosed
        {
            get;
            set;
        }
    }
}
