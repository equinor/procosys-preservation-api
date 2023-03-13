using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    [Obsolete("Revert to use Topic-classes from Equinor.ProCoSys.PcsServiceBus after ensuring real Guids on bus.")]
    public class McPkgTmpTopic
    {
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

        public string ProjectName
        {
            get;
            set;
        }

        public string Behavior
        {
            get;
            set;
        }

        public string CommPkgNo
        {
            get;
            set;
        }

        public string CommPkgNoOld
        {
            get;
            set;
        }

        public string McPkgNo
        {
            get;
            set;
        }

        public string McPkgId
        {
            get;
            set;
        }

        public string McPkgNoOld
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

        public string Discipline
        {
            get;
            set;
        }

        public string McStatus
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

        public string CreatedAt
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
