using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductFilterService.Models
{
    public class FilterResult
    {
        public List<Product> Products { get; set; }
        public FilterObject Filter { get; set; }
    }
}
