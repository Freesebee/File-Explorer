namespace Lab1
{
    public enum SortBy
    {
        Alphabetic,
        Size,
        Extension,
        Date,
    }

    public class SortOptions : ViewModelBase
    {
        public SortBy SortType
        {
            get => SortType;
            set
            {
                if (SortType != value)
                {
                    SortType = value;
                    NotifyPropertyChanged(nameof(SortType));
                }
            }
        }


        public SortOrder Direction
        {
            get => Direction;
            set
            {
                if (Direction != value)
                {
                    Direction = value;
                    NotifyPropertyChanged(nameof(Direction));
                }
            }
        }
    }
}
