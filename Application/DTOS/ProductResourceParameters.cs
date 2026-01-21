using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class ProductResourceParameters
    {
        //Pagination Parameters
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        //filtering Parameters
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }

        //Sorting Parameters
        public string? OrderBy { get; set; }
    }
}
