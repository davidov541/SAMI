using System;
using System.ComponentModel;
using System.Windows;

namespace SAMI.Application.LogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            Database = new EntryDatabase();
            DataContext = (Object) Database;
            InitializeComponent();
            Database.Refresh();
        }

        internal EntryDatabase Database
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Database.Refresh();
        }
    }
}
