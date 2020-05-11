namespace Equinor.Procosys.Preservation.Domain
{
    public class AttachmentOptions
    {
        public int MaxSizeKb { get; set; }
        public string BlobContainer { get; set; }
        public int BlobClockSkewMinutes { get; set; }
        public string[] ValidFileSuffixes { get; set; }
    }
}
