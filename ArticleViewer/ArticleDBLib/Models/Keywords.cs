using System.Collections.Generic;

namespace ArticleDBLib.Models
{
    public class Keywords
    {
        public int Id { get; set; }
        public string Keyword { get; set; }
        public virtual  List<Articles> Articles { get; set; }
        public string GetKeyword => $"{Keyword}";
        //jeśli niepotrzebne, usunąć
        public int GetKeywordId => Id;
    }
}
