namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class Sorting
    {
        public Sorting(SortingDirection sortingDirection, SortingColumn sortingColumn)
        {
            SortingDirection = sortingDirection;
            SortingColumn = sortingColumn;
        }

        public SortingDirection SortingDirection { get;  }
        public SortingColumn SortingColumn { get;  }
    }
}
