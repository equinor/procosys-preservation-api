﻿using System;

namespace Equinor.Procosys.Preservation.Query.GetTagActionDetails
{
    public class ActionDetailsDto
    {
        public ActionDetailsDto(
            int id,
            PersonDto createdBy,
            DateTime createdAt,
            string title,
            string description,
            DateTime? dueTimeUtc,
            bool isClosed,
            PersonDto closedBy,
            DateTime? closedAtUtc,
            ulong rowVersion)
        {
            Id = id;
            CreatedBy = createdBy;
            CreatedAtUtc = createdAt;
            Title = title;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            ClosedBy = closedBy;
            ClosedAtUtc = closedAtUtc;
            RowVersion = rowVersion;
        }


        public int Id { get; }
        public PersonDto CreatedBy { get; }
        public DateTime CreatedAtUtc { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public PersonDto ClosedBy { get; }
        public DateTime? ClosedAtUtc { get; }
        public ulong RowVersion { get; }
    }
}
