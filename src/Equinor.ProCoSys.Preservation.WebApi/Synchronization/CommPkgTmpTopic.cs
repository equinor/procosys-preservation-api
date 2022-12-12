using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class CommPkgTmpTopic
    {
        public const string TopicName = "commpkg";

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

        public string ProjectNameOld
        {
            get;
            set;
        }

        public string CommPkgNo
        {
            get;
            set;
        }

        public string CommPkgId
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string PlantName
        {
            get;
            set;
        }

        public string DescriptionOfWork
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        public string ResponsibleCode
        {
            get;
            set;
        }

        public string ResponsibleDescription
        {
            get;
            set;
        }

        public string AreaCode
        {
            get;
            set;
        }

        public string AreaDescription
        {
            get;
            set;
        }

        public string Phase
        {
            get;
            set;
        }

        public string CommissioningIdentifier
        {
            get;
            set;
        }

        public bool IsVoided
        {
            get;
            set;
        }

        public bool Demolition
        {
            get;
            set;
        }

        public string CreatedAt
        {
            get;
            set;
        }

        public string Priority1
        {
            get;
            set;
        }

        public string Priority2
        {
            get;
            set;
        }

        public string Priority3
        {
            get;
            set;
        }

        public List<string> ProjectNames
        {
            get;
            set;
        }

        public string LastUpdated
        {
            get;
            set;
        }
    }
}
