namespace ArticleDBLib.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public int Id_Article { get; set; }
        public virtual Articles Article { get; set; }
        public string Commenter { get; set; }
        public string Comment { get; set; }
        public string GetComment => $"{Commenter} says: {Comment}";
    }
}
