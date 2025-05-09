using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CustomEntities
{
    public class PaginationOptions
    {
        public int DefaultPageSize { get; set; } = 12;
        public int DefaultPageNumber { get; set; } = 1;
    }
}
