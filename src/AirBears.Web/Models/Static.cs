using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AirBears.Web.Models
{
    public static class Static
    {
        public static Expression<Func<User, object>> GetSortExpression(string sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                //default sort by last name.
                return u => u.LastName;
            }
            else if (sortBy.Equals("firstName", StringComparison.CurrentCultureIgnoreCase))
            {
                return u => u.FirstName;
            }
            else if (sortBy.Equals("lastName", StringComparison.CurrentCultureIgnoreCase))
            {
                return u => u.LastName;
            }
            else if (sortBy.Equals("dateRegistered", StringComparison.CurrentCultureIgnoreCase))
            {
                return u => u.DateRegistered;
            }

            return u => u.LastName;
        }
    }
}
