﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AirBears.Web.Models
{
    public static class Extenstions
    {
        /// <summary>
        /// Returns all of the user's Role claims or and empty collection if there are none.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
        {
            var roles = principal.FindAll(ClaimTypes.Role);

            return roles.Any() ? roles.Select(r => r.Value) : Enumerable.Empty<string>();
        }
    }
}
