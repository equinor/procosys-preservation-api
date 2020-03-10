namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class Sorting
    {
        public Sorting(SortingDirection direction, SortingColumn column)
        {
            Direction = direction;
            Column = column;
        }

        public SortingDirection Direction { get;  }
        public SortingColumn Column { get;  }
    }
}
