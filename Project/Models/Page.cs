using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestProject.Services;

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
        
        [Display(Name = "Min RT")]
        public double? MinResponseTime {get;set;}

        [Display(Name = "Max RT")]        
        public double? MaxResponseTime {get;set;}        

        [Display(Name = "Sitemap")]
        public int SId {get;set;}

        public static List<Page> toPage(IEnumerable<string> list)
        { 
            List<Page> sitemaps = new List<Page>();  
            foreach(var item in list)
            {
                sitemaps.Add(new Page() { PageLink = TestProject.Services.UrlFormater.GetRightPart(item) });
            }
            return sitemaps;
        }

        public virtual Sitemap Sitemap {get;set;}
    }
}