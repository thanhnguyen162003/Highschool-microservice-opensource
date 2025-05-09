using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.QueryFilters
{
    public class UniversityQueryFilter
    {
        public string? Search { get; set; }
        public Guid? UniversityId { get; set; }
        public string? MajorCode { get; set; }
        public double? MinTuition { get; set; }
        public double? MaxTuition { get; set; }
        public int? City { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
