using ArticleDBLib;
using ArticleDBLib.Models;
using System.Windows;
using System.Windows.Controls;

namespace ArticleViewer
{
    /// <summary>
    /// Interaction logic for Group.xaml
    /// </summary>
    public partial class Group : Window
    {
        public static Keywords SelectedKeyword { get; set; }

        public void LoadKeywordsToList()
        {
            ListOfKeywords.ItemsSource = DbDataAccess.GetKeywords();
        }

        public void LoadGroupedArticlesToList()
        {
            MainWindow.AppWindow.lstBox.ItemsSource = DbDataAccess.SortArticlesByKeywords(SelectedKeyword.Id);
        }
 
        public Group()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            LoadKeywordsToList();
        }

        private void KeyList_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;

            SelectedKeyword = (Keywords)item.SelectedItem;
            
        }

        private void GroupByBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadGroupedArticlesToList();
            this.Close();
        }


    }
}
