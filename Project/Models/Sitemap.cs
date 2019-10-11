using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    [Table("Sitemaps")]
    public class Sitemap
    {
        [Key]
        [Display(Name = "ID")]
        public int SId {get;set;}
        
        [Display(Name = "Sitemap link")]
        [StringLength(2048)]
        public string SitemapLink {get;set;}
        
        [Display(Name = "WebAdress")]
        public int UId {get;set;}

        public static List<Sitemap> toSitemap(List<string> list,int lenght)
        {
            List<Sitemap> sitemaps = new List<Sitemap>();  
            foreach(var item in list)
            {
                sitemaps.Add(new Sitemap() { SitemapLink = item.Substring(lenght) });
            }
            return sitemaps;
        }

        public virtual WebAddress WebAddress {get;set;}
        public virtual ICollection<Page> Pages {get;set;}
    }
}