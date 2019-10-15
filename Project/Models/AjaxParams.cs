using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class AjaxRequest
    {
        public string url {get;set;}
        public int currentPage {get;set;}
        public int numToTake {get;set;}
    }

    public class AjaxResponse
    {
        public string url {get;set;}
        public bool isSuccess {get;set;} = false;
        public string htmlResult {get;set;} = "";
    }
}