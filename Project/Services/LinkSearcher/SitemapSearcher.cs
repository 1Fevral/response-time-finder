using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Services
{
    public class SitemapSearcher
    {
        private WebAddress wa;
        public WebAddress webAddress { get { return wa;} }

        public SitemapSearcher (WebAddress wa) {
            this.wa = wa;
        }

        public void FindIn(string rightPart) 
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(this.wa.Url + rightPart);
                StreamReader reader = new StreamReader(stream);
                
                if(reader.EndOfStream) {
                    return;
                }
                
                string line;
                this.wa.Sitemaps = new List<Sitemap>();

                while(!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if(line.Contains("Sitemap")) 
                    {
                        string link = line.Split(":",2)[1].Trim();
                        this.wa.Sitemaps.Add(new Sitemap{
                            SitemapLink = UrlFormater.GetRightPart(link)
                        });
                    }
                }
                if(this.wa.Sitemaps.Count > 0) wa.isSuccess = true;
            }
            catch {}
        }
    }
}