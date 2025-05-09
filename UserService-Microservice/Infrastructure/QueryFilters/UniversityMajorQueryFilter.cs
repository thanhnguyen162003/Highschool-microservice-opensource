using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.QueryFilters
{
    public class UniversityMajorQueryFilter
    {
        public string? Search { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

}
