using System.Linq;
using System.Text.RegularExpressions;

namespace TestProject.Services
{
    public static class UrlFormater
    {
        public static string Formate(string link)
        {
            if(link.Count(x => x == '/') < 3) link += "/";
            Regex regex = new Regex(@"https?:\/\/([^\/]+)\/");
            Match match = regex.Match(link);

            if (match.Success) return match.Groups[0].Value ;
      
            return link;
        }
    }
}