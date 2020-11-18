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
        public static string RequirementTypeCode => "TestRT";
        public static string RequirementTypeDescription => "Test - RequirementType";
        public static string RequirementDefinition => "TestRD";
        public static string JourneyA => "TestJourneyA";
        public static string StepInJourneyA => "TestStepA";
        public static string JourneyB => "TestJourneyB";
        public static string StepInJourneyB => "TestStepB";
        public static string StandardTagNo => "Std-Test";
        public static string StandardTagDescription => "Test - StdTag";
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
        public List<int> ModeIds = new List<int>();
        public List<int> StandardTagActionIds = new List<int>();
        public List<int> SiteAreaTagActionIds = new List<int>();
        public List<int> StandardTagAttachmentIds = new List<int>();
        public List<int> SiteAreaTagAttachmentIds = new List<int>();
        public List<int> StandardTagActionAttachmentIds = new List<int>();
        public List<int> SiteAreaTagActionAttachmentIds = new List<int>();
    }
}
