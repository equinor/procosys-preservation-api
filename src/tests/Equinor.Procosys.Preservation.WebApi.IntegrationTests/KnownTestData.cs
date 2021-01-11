using System;

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
        public static string ReqDefInReqTypeAWithInfoField => "TestRD-A-Info";
        public static string ReqDefInReqTypeAWithCbField => "TestRD-A-Cb";
        public static string ReqDefInReqTypeAWithAttachmentField => "TestRD-A-Attachment";
        public static string ReqTypeB => "TestRT-B";
        public static string ReqDefInReqTypeB => "TestRD-B";
        public static string JourneyWithTags => "TestJourneyA";
        public static string StepAInJourneyWithTags => "TestStepA";
        public static string StepBInJourneyWithTags => "TestStepB";
        public static string JourneyNotInUse => "TestJourneyB";
        public static string StepInJourneyNotInUse => "TestStepB";
        public static string StandardTagNo => "Std-Test";
        public static string SiteTagNo => "#SITE-Test";
        public static string SiteTagDescription => "Test - SiteTag";
        public static string Action => "TestAction";
        public static string ActionDescription => "Test - Action";
        public static Guid ActionAttachmentBlobStorageId = new Guid("{11111111-1111-1111-1111-111111111111}");
        public static Guid TagAttachmentBlobStorageId = new Guid("{22222222-2222-2222-2222-222222222222}");

        public int TagId_ForStandardTagReadyForBulkPreserve_NotStarted;
        public int TagId_ForStandardTagWithAttachmentsAndActionAttachments_Started;
        public int TagId_ForStandardTagWithInfoRequirement_Started;
        public int TagId_ForStandardTagWithCbRequirement_Started;
        public int TagId_ForStandardTagWithAttachmentRequirement_Started;
        public int TagId_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
        public int TagId_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted;
        public int ModeId;
    }
}
