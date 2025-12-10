using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    [Obsolete("Revert to use Topic-classes from Equinor.ProCoSys.PcsServiceBus after ensuring real Guids on bus.")]
    // Clarification: "real Guids on bus" refers to the ProCoSysGuid message property in the service bus messages.
    // Currently (2024-11-28) they look something like this; EB38CCCAAE98D926E0532810000AC5B2,
    // instead of a real Guid like 123e4567-e89b-12d3-a456-426614174000.
    // The Equinor.ProCoSys.PcsServiceBus classes contain a ProCoSysGuid property of type Guid.
    // When we try to deserialize the current ProCoSysGuid message property to a Guid, we get an exception.
    // This Tmp class mirrors the properties of the original class, but with ProCoSysGuid as a string.
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
