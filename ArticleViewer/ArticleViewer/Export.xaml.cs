using System.Collections.Generic;
using System.Text;
using System.Windows;
using ArticleDBLib;
using ArticleDBLib.Models;
using ListBox = System.Windows.Controls.ListBox;
using Microsoft.Win32;
using System.IO;

namespace ArticleViewer
{
    /// <summary>
    /// Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : Window
    {
        public List<Articles> SelectedArticles = new List<Articles>();
        
        
        public static string Retract { get; private set; } = "\t";

        public static string LineFeed { get; private set; } = "\n";

        public static bool Align { get; private set; } = false;
        
        public Export()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            listBox.ItemsSource = DbDataAccess.LoadArticleList();
        }

        private void listBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var items = (ListBox)sender;
            SelectedArticles.Clear();
            foreach (Articles item in listBox.SelectedItems)
            {
                SelectedArticles.Add(item);
            }
        }
        
        private void saveBib_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BibTex file (*.bib)|*.bib";
            saveFileDialog.FileName = "BiBTeXEntry";
            if (saveFileDialog.ShowDialog() == true) 
            { 
                
                File.WriteAllText(saveFileDialog.FileName, ToBib());
            }
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Ustawienie klucza na bazie autora tytulu i roku
        public string GetKey(Articles a)
        {
            var str = DbDataAccess.GetAuthorsOfArticle(a.Id);
            string auth = "";
            foreach (Authors au in str)
            {
                auth = string.Concat(auth, (au.Author.Substring(0, 1)));
            }
            string key = string.Concat(auth, a.Title.Substring(0, 2), a.Year.ToString());
            return key.ToLower();
        }
        //Generowanie pliku bib po pobraniu danych z bazy
        public string ToBib()
        {
            var bib= new StringBuilder("");
            //Wybieranie artukulów do eksportu
            if (listBox.SelectedItems.Count==0)
            {
                MessageBox.Show("Select an article/s!");
            }
            foreach (Articles a in listBox.SelectedItems)
            {
                bib.Append("@article{");
                bib.Append(GetKey(a));
                bib.Append(",");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("author = {");
                var str = DbDataAccess.GetAuthorsOfArticle(a.Id);
                var note = DbDataAccess.GetKeywordsToArticle(a.Id);
                foreach (Authors au in str)
                {
                    if (str.Count !=0)
                    {
                        int lastIndex = str.Count - 1;
                        if (lastIndex == str.IndexOf(au))
                        {
                            bib.Append(au.Author);
                        }
                        else
                        {
                            bib.Append(au.Author + ", ");
                        }
                    }
                }
                bib.Append("},");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("title = {");
                bib.Append(a.Title);
                bib.Append("},");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("journal = {");
                bib.Append(a.Journal);
                bib.Append("},");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("year = ");
                bib.Append(a.Year.ToString());
                bib.Append(",");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("number = ");
                bib.Append(a.Number.ToString());
                bib.Append(",");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("pages = {");
                bib.Append(a.Pages.ToString());
                bib.Append("},");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("volume = ");
                bib.Append(a.Volume.ToString());
                bib.Append(",");
                bib.Append(LineFeed);
                bib.Append(Retract);
                bib.Append("note = {");
                foreach (Keywords k in note)
                {
                    if (note.Count != 0)
                    {
                        int lastIndexK = note.Count - 1;
                        if (lastIndexK == note.IndexOf(k))
                        {
                            bib.Append(k.Keyword);
                        }
                        else
                        {
                            bib.Append(k.Keyword + ", ");
                        }
                    }
                }
                bib.Append("}");
                bib.Append(LineFeed);
                bib.Append("}");
                bib.Append(LineFeed);
            }
            return bib.ToString();
        }
    }
}
