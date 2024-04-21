using System.Windows;

namespace Lab1.Dialogs
{
    /// <summary>
    /// Interaction logic for SortOptionsDialog.xaml
    /// </summary>
    public partial class SortOptionsDialog : Window
    {
        public SortOrder SortOrder { get; private set; }
        public SortBy SortBy { get; private set; }

        public SortOptionsDialog()
        {
            InitializeComponent();
        }

        private void Button_Click_Confirm(object sender, RoutedEventArgs e)
        {
            if ((bool)SortAlphabeticly.IsChecked!)
                SortBy = SortBy.Alphabetic;
            else if ((bool)SortByDate.IsChecked!)
                SortBy = SortBy.Date;
            else if ((bool)SortBySize.IsChecked!)
                SortBy = SortBy.Size;
            else if ((bool)SortByExtension.IsChecked!)
                SortBy = SortBy.Extension;
            else
                throw new NotImplementedException();

            if ((bool)Ascending.IsChecked!)
                SortOrder = SortOrder.Ascending;
            else if ((bool)Descending.IsChecked!)
                SortOrder = SortOrder.Descending;
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
