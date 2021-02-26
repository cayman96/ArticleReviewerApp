using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.IO;
using ArticleDBLib.Models;
using ArticleDBLib;
using System.Windows.Forms;

namespace ArticleViewer
{
    /// <summary>
    /// Interaction logic for Import.xaml
    /// </summary>
    public partial class Import : Window
    {
        private string filePath = string.Empty;
        private List<string> Lines = new List<string>();
        private List<Authors> Auts = new List<Authors>();
        private List<string> Authorss = new List<string>();
        private List<Articles> Articless = new List<Articles>();
        private List<string> Keywordss = new List<string>();
        private List<Keywords> Keys = new List<Keywords>();
        public Import()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            
        }
        public bool CheckSource()
        {
            bool EmptyField = false;
            if(string.IsNullOrWhiteSpace(src.Text))
            {
                src.BorderBrush = Brushes.Red;
                EmptyField = true;
            }
            else
            {
                src.BorderBrush = Brushes.Black;
            }
            // Po wykryciu pustego pola wyświetlany jest komunikat
            if (EmptyField)
            {
                System.Windows.MessageBox.Show("Please choose a file.");
                return true;
            }
            else
            {
                return false;
            }

        }
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "BibTex file (*.bib)|*.bib";
            if (o.ShowDialog().ToString().Equals("OK"))
            {
                filePath = o.FileName;
                src.Text = filePath;
            }
        }
        //Wczytanie pliku do listy stringów
        private void Load(string f)
        {
            using (StreamReader reader = new StreamReader(f))
            {
                string line;
                while ((line=reader.ReadLine()) != null)
                {
                    Lines.Add(line);
                }
            }
        }
        //Sprawdzenie czy jest to BiBTex
        private int Check(List<string> l)
        {
            int count = 0;
            foreach (string line in l)
            {
                if (line.StartsWith("@article"))
                {
                    count++;
                }
            }
            return count;
        }
        //Pocięcie listy na mniejsze(potrzebne, gdy jest kilka BibTexów w jednym pliku
        private void  ListCutter(List<string> l)
        {
            int lineCount = 0;
            int start,stop;
            List<string> temp = new List<string>();
            List<int> articleLines = new List<int>();
            foreach (string line in l)
            {
                if (line.StartsWith("@article", StringComparison.InvariantCultureIgnoreCase))
                {
                    articleLines.Add(lineCount);
                }
                lineCount++;
            }
            articleLines.Add(l.Count);
            //Jak jest jeden artykuł to po prostu wrzuca całą liste
            if (articleLines.Count == 2)
            {
                Upload(l);
            }
            //Jak artykułów jest więcej to tnie liste i po kolei wrzuca Każdy rekord
            else if (articleLines.Count > 2)
            {
                int max = articleLines.Count;
                //petla dla linii z article czyli poczatek nowej listy
                for (int i = 0; i<max-1; i++)
                {
                    start = articleLines[i];
                    stop = articleLines[i + 1];
                    //petla dla dodawania do nowej listy elementow z danego zakresu
                    for(int j=start; j<stop; j++)
                    {
                        string tmp = l[j];
                        temp.Add(tmp);
                    }
                    
                    if (Check(temp) != 0)
                        Upload(temp);
                    else
                        System.Windows.MessageBox.Show("This is not a BibTeX entry! ");
                    temp.Clear();
                    
                }
            }
        } 
       //kod właściwy czyli wyciąganie wartości i na ich podstawie dodanie nowy art do bazy
        private void Upload(List<string> l)
        {
            Authorss.Clear();
            Keywordss.Clear();
            Authors uAuthor = new Authors() { Author = "Not found" };
            string uTitle = "Not found";
            string uJournal = "Not found";
            int uYear = 0000;
            int uNumber = 0;
            int uPages = 0;
            int uVolume = 0;
            string temp;
            Keywords uKeyword = new Keywords() { Keyword = "Not found" };
            //Wyciaganie z pliku wartości potrzebnych do tworzenia nowego obiektu
            foreach (string line in l)
            {
                if (line.StartsWith("\tauthor", StringComparison.InvariantCultureIgnoreCase))
                {
                    temp = line.Substring(line.LastIndexOf("{") + 1, (line.LastIndexOf("}") - line.LastIndexOf("{") - 1));
                    string[] authors = temp.Split(',');
                    foreach (string a in authors) 
                    {
                        
                        if (a != "")
                        {
                            Authorss.Add(a.Trim());
                        }
                    }
                }
                else if (line.StartsWith("\ttitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    uTitle = line.Substring(line.LastIndexOf("{")+1, (line.LastIndexOf("}") - line.LastIndexOf("{")-1));
                }
                else if (line.StartsWith("\tjournal", StringComparison.InvariantCultureIgnoreCase))
                {
                    uJournal = line.Substring(line.LastIndexOf("{")+1, (line.LastIndexOf("}") - line.LastIndexOf("{")-1));
                }
                else if (line.StartsWith("\tyear", StringComparison.InvariantCultureIgnoreCase))
                {
                    uYear = int.Parse(line.Substring(line.LastIndexOf("=")+2,4));
                }
                else if (line.StartsWith("\tnumber", StringComparison.InvariantCultureIgnoreCase))
                {
                    uNumber = int.Parse(line.Substring(line.LastIndexOf("=") + 2,1));
                }
                else if (line.StartsWith("\tpages", StringComparison.InvariantCultureIgnoreCase))
                {
                    uPages = int.Parse(line.Substring(line.LastIndexOf("{")+1, (line.LastIndexOf("}") - line.LastIndexOf("{")-1)));
                }
                else if (line.StartsWith("\tvolume", StringComparison.InvariantCultureIgnoreCase))
                {
                    uVolume = int.Parse(line.Substring(line.LastIndexOf("=") + 2, line.Count() - line.LastIndexOf("=") - 3));
                }
                else if (line.StartsWith("\tnote", StringComparison.InvariantCultureIgnoreCase))
                {
                    temp = line.Substring(line.LastIndexOf("{") + 1, (line.LastIndexOf("}") - line.LastIndexOf("{") - 1));
                    string[] keywords = temp.Split(',');
                    foreach(string k in keywords)
                    {
                        if (k != "")
                        {
                            Keywordss.Add(k.Trim());
                        }
                    }
                }
            }
            Articless = DbDataAccess.LoadArticleList();
            int index = Articless.FindIndex(item => item.Title == uTitle);
            if (index >= 0)
            {
                System.Windows.MessageBox.Show("Article with this title already exist in database!", "File already exist");
            }
            //tworzenie nowego obiektu, analogicznie jak w info
            else
            {
                Articles a = new Articles()
                {
                    Title = uTitle,
                    Journal = uJournal,
                    Year = uYear,
                    Number = uNumber,
                    Pages = uPages,
                    Volume = uVolume
                };
                DbDataAccess.SaveArticle(a);
                
                if(Authorss.Count != 0)
                {
                    foreach (string auth in Authorss)
                    {
                        List<Authors> authorsList = new List<Authors>(DbDataAccess.GetAuthors());
                        int lastIndexx= Authorss.Count()-1;
                        if (authorsList.Exists(x => x.Author == auth))
                        {
                            Authors au = authorsList.Find(x => x.Author == auth);
                            Auts.Add(au);
                        }
                        else
                        {
                            Authors _a = new Authors() { Author = auth };
                            DbDataAccess.SaveAuthor(_a);
                            List<Authors> tempAut = DbDataAccess.GetAuthors();
                            int lastIdx = tempAut.Count - 1;
                            _a = tempAut[lastIdx];
                            Auts.Add(_a);
                        }
                    }

                }
                else
                {
                    Auts.Add(uAuthor);
                }
                
                DbDataAccess.MatchAuthorsToArticles(DbDataAccess.GetLastArticleId(), Auts);
                Auts.Clear();
                if (Keywordss.Count != 0)
                {
                    foreach (string key in Keywordss)
                    {
                        List<Keywords> keywordsList = new List<Keywords>(DbDataAccess.GetKeywords());
                        int lastIndexy= Keywordss.Count()-1;
                        if (keywordsList.Exists(x => x.Keyword == key))
                        {
                            Keywords ke = keywordsList.Find(x => x.Keyword == key);
                            Keys.Add(ke);
                        }
                        else
                        {
                            Keywords _k = new Keywords() { Keyword = key };
                            
                            DbDataAccess.SaveKeyword(_k);
                            List<Keywords> tempKey = DbDataAccess.GetKeywords();
                            int lastIdxk = tempKey.Count - 1;
                            _k = tempKey[lastIdxk];
                            Keys.Add(_k);
                        }
                    }

                }
                else
                {
                    Keys.Add(uKeyword);
                }
                DbDataAccess.MatchKeywordsToArticle(DbDataAccess.GetLastArticleId(), Keys);
                Keys.Clear();
                System.Windows.MessageBox.Show($"Select file to article {uTitle}");
                OpenFileDialog o = new OpenFileDialog();
                o.Filter = "PDF Files | *.pdf";
                if (o.ShowDialog().ToString().Equals("OK"))
                {
                    filePath = o.FileName;
                }
                List<Files> filesList = new List<Files>(DbDataAccess.GetFilesList());
                if (filesList.Exists(x => x.Filename == System.IO.Path.GetFileName(filePath)))
                {
                    System.Windows.MessageBox.Show("The selected article already exists. Please choose another one.");
                    if (o.ShowDialog().ToString().Equals("OK"))
                    {
                        filePath = o.FileName;
                    }
                };
                DbDataAccess.SaveFile(DbDataAccess.GetLastArticleId(), System.IO.Path.GetFileName(filePath));
                MainWindow.AppWindow.LoadArticlesToList();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void import_Click(object sender, RoutedEventArgs e)
        {
            if (CheckSource())
            {
                return;
            }
            Load(filePath);
            if (Lines is null)
            {
                System.Windows.MessageBox.Show("Failed to load the file!","Invalid File");
            }
            
            else if (Check(Lines) != 0)
            {
                ListCutter(Lines);
            }
            else
                System.Windows.MessageBox.Show("File is not an Article BibTeX entry!","Invalid File");
            this.Close();
        }
    }
}
