using ArticleDBLib.Models;
using System.Configuration;
using System.Data.SQLite;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.IO;
using FluentMigrator.Runner;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Forms;

namespace ArticleDBLib
{
    public class DbDataAccess
    {
        //funkcja ładująca connection stringa z pliku app.config w projekcie ArticleViewer.
        private static string LoadConnectionString(string id = "ArticleDb")
        {
            //ConfigurationManager jest wymagany, żeby odczytać plik z innego projektu
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        //stworzenie nowej bazy danych w razie, gdy nie będzie już żadnego istniejącego i wypełnienie go tabelami
        public static void CreateDB()
        {
            string path = System.Environment.CurrentDirectory + "\\ArticleDb.db";
            if (!File.Exists(path)) {
                SQLiteConnection.CreateFile(path);
                var serviceProvider = CreateServices();
                using (var scope = serviceProvider.CreateScope())
                {
                    UpdateDB(scope.ServiceProvider);
                }
                MessageBox.Show("Database has not been found. \nNew database file has been created.", "Database missing");
            }
        }

        //konfiguracja usługi migratora
        private static IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>rb
                    .AddSQLite()
                    .WithGlobalConnectionString(LoadConnectionString())
                    //wymagane, by umożliwiło migrację na pustej bazie danych
                    .ScanIn(typeof(AddLogTable).Assembly).For.Migrations())
                .BuildServiceProvider(false);
        }
        //właściwa migracja
        private static void UpdateDB(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            //migracja
            runner.MigrateUp();
        }
        //operacje na bazie
        //pobranie listy artykułów z bazy.
        public static List<Articles> LoadArticleList()
        {
            string query = "select * from articles";
            //Pobrane wyniki z bazy są zapisywane jako typ obiektów do wyliczenia.
            IEnumerable<Articles> output = Enumerable.Empty<Articles>();
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = cnn.Query<Articles>(query);
                }
            }
            catch(SQLiteException)
            {
                //w razie wyjątku wyświetli się powiadomienie, a program nie zostanie zamknięty
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
            //funkcja ma zwracać listę, dlatego wymagana jest konwersja.
            return output.ToList();
        }
        //pobranie do listy komentarzy odnoszących się do artykułu o podanym id
        public static List<Comments> GetCommentsToArticles(int aid)
        {
            //parametry anonimowe, które zostaną przekazane do zapytania
            var parameters = new { id = aid };
            string query = "select * from comments where id_article=@id";
            IEnumerable<Comments> output = Enumerable.Empty<Comments>();
            try
            { 
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = cnn.Query<Comments>(query,parameters); 
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
            return output.ToList();
        }
       
        //dodanie komentarza do bazy
        public static void AddComment(int id, string name, string content)
        {
            var parameters = new { id_article = id, commenter = name, comment = content };
            string query = "insert into comments (id_article,commenter,comment) values (@id_article, @commenter, @comment)";
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    //tym razem nie query, bo nie chcemy żadnych wyników, tylko execute, ponieważ chcemy
                    //zapisać nowe dane do bazy
                    cnn.Execute(query, parameters);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
        }
        //pobranie listy autorów. Funkcja ta wyorzystana jest, gdy chcemy wybrać z listy odpowiednich autorów i przypisać ich do artykułu.
        public static List<Authors> GetAuthors()
        {
            string query = "select * from authors";
            IEnumerable<Authors> output = Enumerable.Empty<Authors>();
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = cnn.Query<Authors>(query);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.");
            }
            return output.ToList();
        }
        //pobranie listy słów kluczowych. Wykorzystanie funkcji analogiczne do funkcji pobierającej autorów
        public static List<Keywords> GetKeywords()
        {
            string query = "select * from keywords";
            IEnumerable<Keywords> output = Enumerable.Empty<Keywords>();
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = cnn.Query<Keywords>(query);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite Error");
            }
            return output.ToList();
        }
        // pobranie listy plików
        public static List<Files> GetFilesList()
        {
            string query = "select * from files";
            IEnumerable<Files> output = Enumerable.Empty<Files>();
            try
            {
               
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = cnn.Query<Files>(query);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
            return output.ToList();
        }
        // pobranie nazwy pliku
        public static string GetFileName(int aid)
        {
            var parameters = new { id = aid };
            string query = "select filename from files where id_article = @id";
            string output = "";
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = Convert.ToString(cnn.ExecuteScalar(query, parameters));
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.","SQLite error");
            }
            return output;
        }
        //pozostałe funkcje będą podobne do powyższych, chociaż będą dotyczyć relacji wiele do wielu
        //zapis artykułu, na razie bez autorów
        public static void SaveArticle(Articles article)
        {
            string query = "insert into articles(title,journal,year,volume,number,pages) values(@title,@journal,@year,@volume,@number,@pages)";
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute(query, article);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.SaveArticle", "SQLite error");
            }
        }
        //Zapytanie zwraca id ostatniego artykułu
        //pobiera id zapisywanego artykułu, by później przypisać to id do komentarzy, ścieżki pliku, autorów i słów kluczowych.
        //autoinkrementacja pola id w tabeli articles powoduje, że zawsze przypisuje ostatnie (największe do tej pory) id artykułowi
        //wobec tego zapisane zostanie ono do zmiennej, a następnie przekazane jako argument do odpowiednich funkcji
        public static int GetLastArticleId()
        {
            string query = "select max(id) from articles";
            int output = 0;
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    //potrzebna nam jest tylko jedna wartość, dlatego tutaj użyty jest executescalar
                    //pobierze nie jako typ wyliczalny, ale jako pojedynczą wartość
                    output = Convert.ToInt32(cnn.ExecuteScalar(query));
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.GetLastArticleId", "SQLite error");
            }
            return output;
        }
        public static int GetLastAuthorId()
        {
            string query = "select max(id) from authors";
            int output = 0;
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    //potrzebna nam jest tylko jedna wartość, dlatego tutaj użyty jest executescalar
                    //pobierze nie jako typ wyliczalny, ale jako pojedynczą wartość
                    output = Convert.ToInt32(cnn.ExecuteScalar(query));
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.GetLastArticleId", "SQLite error");
            }
            return output;
        }
        //zapis ścieżki pliku wraz z id odpowiadającemu mu artykułu
        public static void SaveFile(int aid, string name)
        {
            string query = "insert into files (id_article,filename) values(@id_article,@filename)";
            var parameters = new { id_article = aid, filename = name };
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute(query, parameters);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.SaveFile", "SQLite error");
            }
        }
        //zapisanie nowego autora w bazie danych
        public static void SaveAuthor(Authors author)
        {
            string query = "insert into authors(author) values (@author)";
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute(query, author);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.SaveAuthor", "SQLite error");
                MessageBox.Show(SQLiteErrorCode.Error.ToString());
            }
        }
        //dodanie nowego słowa kluczowego do bazy
        public static void SaveKeyword(Keywords keyword)
        {
            string query = "insert into keywords(keyword) values (@keyword)";
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute(query, keyword);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.SaveKeyword", "SQLite error");
            }
        }
        //przypisywanie autorów do zapisywanego artykułu
        //uzupełnia tabelę authorsofarticles
        public static void MatchAuthorsToArticles(int artid, List<Authors> auth)
        {
            string query = "insert into authorsofarticles(id_article,id_author) values(@id_article,@id_author)";
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    //dla każdego autora ma zapisać id zapisywanego artykułu
                    foreach (var item in auth)
                    {
                        cnn.Execute(query, new { id_article = artid, id_author = item.Id });
                    }
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.MatchAuthorsToArticles", "SQLite error");
            }
        }
        //przypisanie słów kluczowych do zapisywanego artykułu
        //uzupełnia tabelę keywordstoarticles
        public static void MatchKeywordsToArticle(int artid,List<Keywords> kwrds)
        {
            string query = "insert into keywordstoarticles(id_article,id_keyword) values(@id_article,@id_keyword)";
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    foreach (var item in kwrds)
                    {
                        cnn.Execute(query, new { id_article = artid, id_keyword = item.Id });
                    }
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.MatchKeywordsToArticle", "SQLite error");
            }
        }
        //zapytania zo relacji wiele-do-wielu
        //stosowane są podczas exportu z bazy danych do bibtexa
        public static List<Authors> GetAuthorsOfArticle(int aid) 
        {
            var parameters = new { id = aid };
            string query = "select au.* from articles ar join authorsofarticles aoa on aoa.id_article = ar.id join authors au on au.id = aoa.id_author where ar.id = @id";
            IEnumerable<Authors> output = Enumerable.Empty<Authors>();
            try
            {  
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = cnn.Query<Authors>(query, parameters);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
            return output.ToList();
        }
        public static List<Keywords> GetKeywordsToArticle(int aid)
        {
            string query = "select kw.* from articles ar join keywordstoarticles kta on kta.id_article = ar.id join keywords kw on kw.id = kta.id_keyword where ar.id = @id";
            var parameters = new { id = aid };
            IEnumerable<Keywords> output = Enumerable.Empty<Keywords>();
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    output = cnn.Query<Keywords>(query, parameters);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
            return output.ToList();
        }
        //usunięcie komentarza
        public static void DeleteComment(int aid)
        {
            string query = "delete from comments where id = @id";
            var parameter = new { id = aid };
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute(query, parameter);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
        }
        //Grupowanie artykułów według słów kluczowych
        //pobiera listę artykułów o odpowiednim słowie kluczowym, powinno działać, sprawdzałem na samym sqlu
        public static List<Articles> SortArticlesByKeywords(int kwrdid)
        {
            var parameter = new { kwrd = kwrdid };
            string query = "select ar.* from articles ar join keywordstoarticles kta on kta.id_article = ar.id join keywords kw on kw.id = kta.id_keyword where kw.id = @kwrd ";
            IEnumerable<Articles> output = Enumerable.Empty<Articles>();
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                { 
                    output = cnn.Query<Articles>(query,parameter);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Cannot find corresponding table due to database file being removed or corrupted.", "SQLite error");
            }
            return output.ToList();
        }
    }
}
