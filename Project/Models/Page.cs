using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    [Table("Pages")]
    public class Page
    {
        [Key]
        [Display(Name = "ID")]
        public int PId {get;set;}

        [Display(Name = "Page link")]
        [StringLength(2048)]
        public string PageLink {get;set;}
        
        [Display(Name = "Response Time")]
        public double? ResponseTime {get;set;}

        [Display(Name = "Sitemap")]
        public int SId {get;set;}

        public static List<Page> toPage(List<string> list,int lenght)
        {
            List<Page> sitemaps = new List<Page>();  
            foreach(var item in list)
            {
                sitemaps.Add(new Page() { PageLink = item.Substring(lenght) });
            }
            return sitemaps;
        }

        public virtual Sitemap Sitemap {get;set;}
    }
}