using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductFilterService.Models
{
    public class FilterObject
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<string> Sizes { get; set; }
        public List<string> CommonWords { get; set; }
    }
}
