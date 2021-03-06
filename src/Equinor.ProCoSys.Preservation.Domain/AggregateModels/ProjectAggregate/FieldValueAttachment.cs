﻿using System;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class FieldValueAttachment : Attachment
    {
        protected FieldValueAttachment() : base()
        {
        }

        public FieldValueAttachment(string plant, Guid blobStorageId, string fileName)
            : base(plant, blobStorageId, fileName, "FieldValue")
        {
        }

        public void SetFileName(string fileName) => FileName = fileName;
    }
}
