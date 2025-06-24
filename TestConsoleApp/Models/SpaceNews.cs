using System.Collections.Generic;

namespace TestConsoleApp.Models
{
    public class SpaceNews
    {
        public int count { get; set; }
        public string next { get; set; }
        public List<Article> results { get; set; }
    }
}
