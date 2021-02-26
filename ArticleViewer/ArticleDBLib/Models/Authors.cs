using System.Collections.Generic;

namespace ArticleDBLib.Models
{
    public class Authors
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public virtual List<Articles> Articles { get; set; }
        public string GetAuthor => $"{Author}";
        public int GetAuthorId => Id;
    }
}
