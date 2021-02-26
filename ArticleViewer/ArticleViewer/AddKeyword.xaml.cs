using ArticleDBLib;
using ArticleDBLib.Models;
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

namespace ArticleViewer
{
    /// <summary>
    /// Logika interakcji dla klasy addKeyword.xaml
    /// </summary>
    public partial class AddKeyword : Window
    {
        public bool CheckKeywordField()
        {
            bool EmptyField = false;

            // Lista pól do sprawdzenia
            List<TextBox> textBoxes = new List<TextBox>() { KName };

            // Sprawdzenie czy pole jest puste
            foreach (TextBox tB in textBoxes)
            {
                if (string.IsNullOrWhiteSpace(tB.Text))
                {
                    tB.BorderBrush = Brushes.Red;
                    EmptyField = true;
                }
                else
                {
                    tB.BorderBrush = Brushes.Black;
                }
            }

            if (EmptyField)
            {
                MessageBox.Show("Enter the keyword.");
                return true;
            }
            else
            {
                return false;
            }
        }
        public AddKeyword()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ListOfKeywords.ItemsSource = DbDataAccess.GetKeywords();
        }

        private void ListOfKeywords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = (ListBox)sender;
            Info.SelectedKeywords.Clear();
            foreach (Keywords k in items.SelectedItems)
            {
                Info.SelectedKeywords.Add(k);
            }
        }

        private void AddNewKeywordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckKeywordField())
            {
                return;
            }
            List<Keywords> keywordsList = new List<Keywords>(DbDataAccess.GetKeywords());
            if (keywordsList.Exists(x => x.Keyword == KName.Text))
            {
                MessageBox.Show("Keyword already exist!");
                KName.BorderBrush = Brushes.Red;
                KName.Clear();
                return;
            }
            else
            {
                Keywords k = new Keywords() { Keyword = KName.Text };
                DbDataAccess.SaveKeyword(k);
                ListOfKeywords.ItemsSource = DbDataAccess.GetKeywords();
                KName.Clear();
            }

        }

        private void AddKeywordsToTheArticleBtn_Click(object sender, RoutedEventArgs e)
        {
            int lastIndex = Info.SelectedKeywords.Count - 1;
            Info.AppWindow.Keywords.Clear();
            foreach (Keywords k in Info.SelectedKeywords)
            {
                if (lastIndex == Info.SelectedKeywords.IndexOf(k))
                {
                    Info.AppWindow.Keywords.Text += k.Keyword;
                }
                else
                {
                    Info.AppWindow.Keywords.Text += k.Keyword + ", ";
                }
            }
            this.Close();
        }
    }
}
