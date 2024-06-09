using Lab1.DAL.Entities;
using Lab1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab1.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for MetadataDialog.xaml
    /// </summary>
    public partial class MetadataDialog : Window
    {
        public class MetadataDialogResult
        {
            public FileMetadata Metadata { get; set; }
        }

        public MetadataDialogResult? Result { get; set; }

        public List<FileMetadata> MetadataSource = new();

        public MetadataDialog(List<FileMetadata> metadata)
        {
            InitializeComponent();

            if (!metadata.Any()) metadata.Add(new FileMetadata());
            
            MetadataSource = metadata;
            
            metaDataGrid.ItemsSource = MetadataSource;
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            Result = new() { Metadata = MetadataSource.First() };

            DialogResult = true;
        }
    }
}
