using System;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTags
{
    public class Paging
    {
        public Paging(int page, int size)
        {
            if (page < 0)
            {
                throw new ArgumentException($"{nameof(page)} must be zero or positive");
            }
            if (size <= 0)
            {
                throw new ArgumentException($"{nameof(page)} must be positive");
            }
            Page = page;
            Size = size;
        }

        public int Page { get; }
        public int Size { get; }
    }
}
