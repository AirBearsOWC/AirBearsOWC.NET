using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirBears.Web.ViewModels
{
    public class QueryResult<T> where T : class
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
