using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using ArticleDBLib.Models;
using ArticleDBLib;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ArticleViewer
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        public static List<Authors> SelectedAuthors = new List<Authors>();
        public static List<Keywords> SelectedKeywords = new List<Keywords>();
        BackgroundWorker worker = new BackgroundWorker();
        public static Info AppWindow;

        // Sprawdzenie czy pola nie są puste
        public bool CheckAllFields()
        {
            bool EmptyField = false;

            // Lista pól do sprawdzenia
            List<System.Windows.Controls.TextBox> textBoxes = new List<System.Windows.Controls.TextBox>() { ATitle, TextBoxAuthor, Volume, Journal, Year, Pages, Number, Keywords, File };
            
            // Sprawdza czy pole jest puste
            foreach(System.Windows.Controls.TextBox tB in textBoxes)
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
            
            // Po wykryciu pustego pola wyświetlany jest komunikat
            if (EmptyField)
            {
                System.Windows.MessageBox.Show("Please fill all fields.");
                return true;
            }
            else
            {
                return false;
            }

        }
        

        public Info()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            AppWindow = this;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.DoWork += Worker_DoWork;

        }

        void CopyFile(string source, string dest)
        {
            FileStream fsOut = new FileStream(dest, FileMode.Create);
            FileStream fsIn = new FileStream(source, FileMode.Open);
            byte[] bt = new byte[1048756];
            int readByte;

            while ((readByte = fsIn.Read(bt, 0, bt.Length)) > 0)
            {
                fsOut.Write(bt, 0, readByte);
                worker.ReportProgress((int)(fsIn.Position * 100 / fsIn.Length));
            }
            fsIn.Close();
            fsOut.Close();
            

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                CopyFile(File.Text, Destin.Text);
            }));
        }

        private void SourceDots_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "PDF Files | *.pdf";
            if (o.ShowDialog().ToString().Equals("OK"))
            {
                string destPath = System.Environment.CurrentDirectory + "\\Articles";
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }

                File.Text = o.FileName;
                Destin.Text = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(File.Text));

            }
            // Sprawdzanie czy plik jest już w bazie
            List<Files> filesList = new List<Files>(DbDataAccess.GetFilesList());
            if (filesList.Exists(x => x.Filename == System.IO.Path.GetFileName(File.Text)))
            {
                System.Windows.MessageBox.Show("The selected article already exists. Please choose another one.");
                File.Clear();
                Destin.Clear();
            };
        }

        private void SaveInfo_Click(object sender, RoutedEventArgs e)
        {
            // Po wykryciu pustego pola zwracana jest wartość true, a następnie zostaje przerwane wykonywanie dalszych instrukcji
            if (CheckAllFields())
            {
                return;
            }
            worker.RunWorkerAsync();
            System.Windows.MessageBox.Show("Article added successfully.");

            Articles a = new Articles()
            {
                Title = ATitle.Text,
                Volume = int.Parse(Volume.Text),
                Journal = Journal.Text,
                Year = int.Parse(Year.Text),
                Pages = int.Parse(Pages.Text),
                Number = int.Parse(Number.Text),

            };
            DbDataAccess.SaveArticle(a);
            DbDataAccess.SaveFile(DbDataAccess.GetLastArticleId(), System.IO.Path.GetFileName(File.Text));
            DbDataAccess.MatchAuthorsToArticles(DbDataAccess.GetLastArticleId(),SelectedAuthors);
            DbDataAccess.MatchKeywordsToArticle(DbDataAccess.GetLastArticleId(), SelectedKeywords);
            MainWindow.AppWindow.LoadArticlesToList();
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ATitle.Clear();
            Volume.Clear();
            Journal.Clear();
            Year.Clear();
            Pages.Clear();
            Number.Clear();
            File.Clear();
            Destin.Clear();
            TextBoxAuthor.Clear();
            SelectedAuthors.Clear();

        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var AddAuthorWindow = new AddAuthor();
            AddAuthorWindow.Show();
        }

        private void AddKeywords_Click(object sender, RoutedEventArgs e)
        {
            var AddKeywordWindow = new AddKeyword();
            AddKeywordWindow.Show();
        }

        private void CheckInt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
