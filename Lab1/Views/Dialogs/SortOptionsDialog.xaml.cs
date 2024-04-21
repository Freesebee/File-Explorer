using System.Windows;

namespace Lab1.Dialogs
{
    /// <summary>
    /// Interaction logic for SortOptionsDialog.xaml
    /// </summary>
    public partial class SortOptionsDialog : Window
    {
        public SortOptions SortOptions { get; private set; }

        public SortOptionsDialog(SortOptions? sortOptions = null)
        {
            InitializeComponent();
            SortOptions = sortOptions ?? new();
            DataContext = SortOptions;
        }

        private void Button_Click_Confirm(object sender, RoutedEventArgs e)
        {
            if ((bool)SortAlphabeticly.IsChecked!)
                SortOptions.SortBy = SortBy.Alphabetic;
            else if ((bool)SortByDate.IsChecked!)
                SortOptions.SortBy = SortBy.Date;
            else if ((bool)SortBySize.IsChecked!)
                SortOptions.SortBy = SortBy.Size;
            else if ((bool)SortByExtension.IsChecked!)
                SortOptions.SortBy = SortBy.Extension;
            else
                throw new NotImplementedException();

            if ((bool)Ascending.IsChecked!)
                SortOptions.Direction = SortOrder.Ascending;
            else if ((bool)Descending.IsChecked!)
                SortOptions.Direction = SortOrder.Descending;
            else
                throw new NotImplementedException();

            DialogResult = true;
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
