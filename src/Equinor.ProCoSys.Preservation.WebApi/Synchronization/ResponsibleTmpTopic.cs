namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class ResponsibleTmpTopic
    {
        public const string TopicName = "responsible";

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

        public string ResponsibleGroup
        {
            get;
            set;
        }

        public string ResponsibleGroupOld
        {
            get;
            set;
        }

        public string Description
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
    }
}
