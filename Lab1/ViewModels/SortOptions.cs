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
        public SortOptions() : base()
        {
            _sortBy = SortBy.Alphabetic;
            _direction = SortOrder.Ascending;
        }

        public SortBy SortBy
        {
            get => _sortBy;
            set
            {
                if (_sortBy != value)
                {
                    _sortBy = value;
                    NotifyPropertyChanged(nameof(SortBy));
                }
            }
        }
        private SortBy _sortBy;


        public SortOrder Direction
        {
            get => _direction;
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    NotifyPropertyChanged(nameof(Direction));
                }
            }
        }
        private SortOrder _direction;
    }
}
