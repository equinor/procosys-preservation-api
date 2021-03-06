﻿using System;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class ActionDetailsDto
    {
        public int Id { get; set; }
        public PersonDto CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueTimeUtc { get; set; }
        public bool IsClosed { get; set; }
        public PersonDto ClosedBy { get; set; }
        public DateTime? ClosedAtUtc { get; set; }
        public int AttachmentCount { get; set; }
        public PersonDto ModifiedBy { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public string RowVersion { get; set; }
    }
}
