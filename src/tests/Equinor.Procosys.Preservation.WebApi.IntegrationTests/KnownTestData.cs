using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public class KnownTestData
    {
        public static string Plant => "PCS$PLANT1";
        public static string ProjectName => "TestProject";
        public static string ProjectDescription => "Test - Project";
        public static string Mode => "TestMode";
        public static string ResponsibleCode => "TestResp";
        public static string ResponsibleDescription => "Test - Responsible";
        public static string ReqTypeA => "TestRT-A";
        public static string ReqDefInReqTypeANoField => "TestRD-A";
        public static string ReqDefInReqTypeAWithAttachmentField => "TestRD-A-Attachment";
        public static string ReqTypeB => "TestRT-B";
        public static string ReqDefInReqTypeB => "TestRD-B";
        public static string JourneyWithTags => "TestJourneyA";
        public static string StepInJourneyWithTags => "TestStepA";
        public static string JourneyNotInUse => "TestJourneyB";
        public static string StepInJourneyNotInUse => "TestStepB";
        public static string StandardTagNo => "Std-Test";
        public static string SiteTagNo => "#SITE-Test";
        public static string SiteTagDescription => "Test - SiteTag";
        public static string Action => "TestAction";
        public static string ActionDescription => "Test - Action";
        public static Guid ActionAttachmentBlobStorageId = new Guid("{11111111-1111-1111-1111-111111111111}");
        public static Guid TagAttachmentBlobStorageId = new Guid("{22222222-2222-2222-2222-222222222222}");

        public List<int> StandardTagIds = new List<int>();
        public List<int> SiteAreaTagIds = new List<int>();
        public List<int> ModeIds = new List<int>();
        public List<int> StepIds = new List<int>();
        public List<int> StandardTagActionIds = new List<int>();
        public List<int> SiteAreaTagActionIds = new List<int>();
        public List<int> StandardTagAttachmentIds = new List<int>();
        public List<int> SiteAreaTagAttachmentIds = new List<int>();
        public List<int> StandardTagActionAttachmentIds = new List<int>();
        public List<int> SiteAreaTagActionAttachmentIds = new List<int>();
    }
}
