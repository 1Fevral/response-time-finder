using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class AjaxParams
    {
        public string url {get;set;}
        public int currentPage {get;set;}
        public int numToTake {get;set;}
    }
}