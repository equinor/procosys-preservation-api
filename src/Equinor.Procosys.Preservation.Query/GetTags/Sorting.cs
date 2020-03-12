namespace Equinor.Procosys.Preservation.Query.GetTags
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
