using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public class AttachmentOptions
    {
        public int MaxSizeKb { get; set; }
        public string BlobContainer { get; set; }
        public string ValidFileSuffixes { get; set; }
        public string[] ValidFileSuffixArray =>
            ValidFileSuffixes != null ? ValidFileSuffixes.ToLower().Split('|', StringSplitOptions.RemoveEmptyEntries) : new string[0];
    }
}
