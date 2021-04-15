namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries
{
    public class Sorting
    {
        public Sorting(SortingDirection direction, SortingProperty property)
        {
            Direction = direction;
            Property = property;
        }

        public SortingDirection Direction { get;  }
        public SortingProperty Property { get;  }
    }
}
