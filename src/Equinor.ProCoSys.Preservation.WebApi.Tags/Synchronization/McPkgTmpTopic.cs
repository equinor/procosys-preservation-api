﻿using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Synchronization
{
    [Obsolete("Revert to use Topic-classes from Equinor.ProCoSys.PcsServiceBus after ensuring real Guids on bus.")]
    // Clarification: "real Guids on bus" refers to the ProCoSysGuid message property in the service bus messages.
    // Currently (2024-11-28) they look something like this; EB38CCCAAE98D926E0532810000AC5B2,
    // instead of a real Guid like 123e4567-e89b-12d3-a456-426614174000.
    // The Equinor.ProCoSys.PcsServiceBus classes contain a ProCoSysGuid property of type Guid.
    // When we try to deserialize the current ProCoSysGuid message property to a Guid, we get an exception.
    // This Tmp class mirrors the properties of the original class, but with ProCoSysGuid as a string.
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
