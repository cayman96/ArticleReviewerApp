using System;
using System.Collections.Generic;

namespace ArticleDBLib.Models
{
    public class Articles
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Journal { get; set; }
        public int? Volume { get; set; }
        public int? Number { get; set; }
        public int? Pages { get; set; }
        public virtual List<Authors> Authors { get; set; } 
        public virtual List<Keywords> Keywords { get; set; }
        public virtual List<Comments> Comments { get; set; }
        public virtual Files File { get; set; }
        public int GetArticleId => Id;
        public string ShowArticleInfo => $"{Title}, {Journal}, {Year}, {Volume},{Number},{Pages}";
    }
}
