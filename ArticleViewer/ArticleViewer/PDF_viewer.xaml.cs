using System.Windows;

namespace ArticleViewer
{
    /// <summary>
    /// Interaction logic for PDF_viewer.xaml
    /// </summary>
    public partial class PDF_viewer : Window
    {
        public PDF_viewer()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            pdfViewerControl.Load(System.Environment.CurrentDirectory + "\\Articles\\" + MainWindow.SelectedArticle.File.Filename);
        }
    }
}
