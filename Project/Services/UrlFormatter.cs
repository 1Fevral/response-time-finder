using System.Linq;
using System.Text.RegularExpressions;

namespace TestProject.Services
{
    public static class UrlFormater
    {
        public static string GetLeftPart(string link)
        {
            if(link.Count(x => x == '/') < 3) link += "/";
            Regex regex = new Regex(@"https?:\/\/([^\/]+)\/");
            Match match = regex.Match(link);

            if (match.Success) return match.Groups[0].Value ;
      
            return link;
        }

        public static string GetLeftPartWOHttp(string link)
        {
            if(link.Count(x => x == '/') < 3) link += "/";
            Regex regex = new Regex(@"https?:\/\/([^\/]+)\/");
            Match match = regex.Match(link);

            if (match.Success) return match.Groups[0].Value.Substring(5);
      
            return link;
        }

        public static string GetRightPart(string link)
        {
            if(link.Count(x => x == '/') < 3) link += "/";
            Regex regex = new Regex(@"https?:\/\/([^\/]+)\/");
            Match match = regex.Match(link);

            if (match.Success) return link.Substring(match.Groups[0].Value.Count());
      
            return link;
        }
    }
}