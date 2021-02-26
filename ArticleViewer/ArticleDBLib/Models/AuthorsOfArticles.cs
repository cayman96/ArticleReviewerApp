using System.Collections.Generic;
using System.Windows.Documents;

namespace ArticleDBLib.Models
{
    public class AuthorsOfArticles
    {
        public int Id_Article { get; set; }
        public virtual List<Articles> Articles { get; set; }
        public int Id_Author { get; set; }
        public virtual List<Authors> Authors { get; set; }
    }
}
