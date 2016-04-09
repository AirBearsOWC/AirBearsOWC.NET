using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirBears.Web.ViewModels
{
    public abstract class QueryBase
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
