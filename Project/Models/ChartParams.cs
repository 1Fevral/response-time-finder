using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class ChartParams
    {
        public List<string> links {get;set;}
        public List<double?> values {get;set;}
        
    }
}