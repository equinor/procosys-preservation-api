﻿using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class TagAttachment : AttachmentBase
    {
        protected TagAttachment() : base()
        {
        }

        public TagAttachment(string plant, string title, Guid blobStorageId)
            : base(plant, title, blobStorageId)
        {
        }
    }
}
