using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    [Table("WebAddresses")]
    public class WebAddress
    {
        [Key]
        [Display(Name = "ID")]
        public int UId {get;set;}

        [Url]
        [Display(Name = "URL")]
        public string Url {get;set;}

        [Display(Name = "IsSuccess")]
        public bool isSuccess {get;set;} = false;

        public virtual ICollection<Sitemap> Sitemaps {get;set;}
    }
}