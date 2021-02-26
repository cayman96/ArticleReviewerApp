using System.Collections.Generic;

namespace ArticleDBLib.Models
{
    public class KeywordsToArticles
    {
        public int Id_Article { get; set; }
        public virtual List<Articles> Articles { get; set; }
        public int Id_Keyword { get; set; }
        public virtual List<Keywords> Keywords { get; set; }
    }
}
