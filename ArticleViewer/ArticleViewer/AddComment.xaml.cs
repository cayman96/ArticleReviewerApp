using ArticleDBLib;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArticleViewer
{
    /// <summary>
    /// Logika interakcji dla klasy AddComment.xaml
    /// </summary>
    public partial class AddComment : Window
    {
        public bool CheckCommentField()
        {
            bool EmptyField = false;

            // Lista pól do sprawdzenia
            List<TextBox> textBoxes = new List<TextBox>() { CName, Message };

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
                MessageBox.Show("Please fill all fields.");
                return true;
            }
            else
            {
                return false;
            }
        }
        public AddComment()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void AddComm_Click(object sender, RoutedEventArgs e)
        {
            if (CheckCommentField())
            {
                return;
            }
            DbDataAccess.AddComment(MainWindow.SelectedArticle.Id, CName.Text, Message.Text);
            MainWindow.AppWindow.LoadCommentToList();
            this.Close();
        }
    }
}
