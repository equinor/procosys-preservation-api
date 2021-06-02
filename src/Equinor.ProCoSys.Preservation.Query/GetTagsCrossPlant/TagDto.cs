namespace Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant
{
    public class TagDto
    {
        public TagDto(
            string plantId,
            string plantTitle,
            string projectName,
            string projectDescription,
            int tagId,
            string tagNo,
            int attachmentCount)
        {
            PlantId = plantId;
            PlantTitle = plantTitle;
            ProjectName = projectName;
            ProjectDescription = projectDescription;
            Id = tagId;
            TagNo = tagNo;
            AttachmentCount = attachmentCount;
        }

        public string PlantId { get; }
        public string PlantTitle { get; }
        public string ProjectName { get; }
        public string ProjectDescription { get; }
        public int Id { get; }
        public string TagNo { get; }
        public int AttachmentCount { get; }
    }
}
