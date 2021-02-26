namespace ArticleDBLib.Models
{
    public class Files
    {
        public int Id_Article { get; set; }
        public virtual Articles Article { get; set; }
        public string Filename { get; set; }
    }
}
