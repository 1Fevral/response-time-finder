using System.Text.RegularExpressions;

namespace TestProject.Services
{
    public static class UrlFormater
    {
        public static string Formate(string link)
        {
            Regex regex = new Regex(@"https?:\/\/([a-zA-Z0-9]+\.?)+");
            Match match = regex.Match(link);

            if (match.Success)
            {
                return match.Groups[0].Value + "/";
            }      
            return link;
        }
    }
}