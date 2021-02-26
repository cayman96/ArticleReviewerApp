using ArticleDBLib;
using ArticleDBLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logika interakcji dla klasy addAuthor.xaml
    /// </summary>
    public partial class AddAuthor : Window
    {
        public bool CheckAuthorField()
        {
            bool EmptyField = false;

            // Lista pól do sprawdzenia
            List<TextBox> textBoxes = new List<TextBox>() { AName };

            // Sprawdza czy pole jest puste
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
                MessageBox.Show("Enter the author's name.");
                return true;
            }
            else
            {
                return false;
            }
        }
    
        public AddAuthor()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ListOfAutors.ItemsSource = DbDataAccess.GetAuthors();
        }

        private void AddAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAuthorField())
            {
                return;
            }
            List<Authors> authorsList = new List<Authors>(DbDataAccess.GetAuthors());
            if(authorsList.Exists(x=>x.Author ==AName.Text))
            {
                MessageBox.Show("Author already exist!");
                AName.BorderBrush = Brushes.Red;
                AName.Clear();
                return;
            }
            else
            {
                Authors au = new Authors() { Author = AName.Text };
                DbDataAccess.SaveAuthor(au);
                ListOfAutors.ItemsSource = DbDataAccess.GetAuthors();
                AName.Clear();
            }
        }
        private void ListOfAutors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = (ListBox)sender;
            Info.SelectedAuthors.Clear();
            foreach(Authors a in items.SelectedItems)
            {
                Info.SelectedAuthors.Add(a);
            }
        }

        private void AddAuthorsToTheArticleBtn_Click(object sender, RoutedEventArgs e)
        {
            int lastIndex = Info.SelectedAuthors.Count - 1;
            Info.AppWindow.TextBoxAuthor.Clear();
            foreach (Authors a in Info.SelectedAuthors)
            {
                if (lastIndex == Info.SelectedAuthors.IndexOf(a))
                {
                    Info.AppWindow.TextBoxAuthor.Text += a.Author;
                }
                else
                {
                    Info.AppWindow.TextBoxAuthor.Text += a.Author + ", ";
                }
            }
            this.Close();
        }
    }
}
