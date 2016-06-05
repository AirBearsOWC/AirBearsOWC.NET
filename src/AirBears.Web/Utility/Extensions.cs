using AirBears.Web.Models;
using AutoMapper;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AirBears.Web
{
    public static class Extensions
    {
        public static string ToHtmlWhiteSpace(this string src)
        {
            return src.Replace("\n", "<br />");
        }

        /// <summary>
        /// Returns all of the user's Role claims or an empty collection if there are none.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
        {
            var roles = principal.FindAll(ClaimTypes.Role);

            return roles.Any() ? roles.Select(r => r.Value) : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name="val">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }

        /// <summary>
        /// Includes all desired pilot navigation properties and filters out authority users.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static IQueryable<User> AsPilots(this DbSet<User> users)
        {
            return users
                .Include(u => u.TeeShirtSize)
                .Include(u => u.State)
                .Include(u => u.FlightTime)
                .Include(u => u.Payload)
                .Where(u => !u.IsAuthorityAccount);
        }

        /// <summary>
        /// Ignores fields that exist on the destination type but not on the source type.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            foreach (var property in expression.TypeMap.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }

            return expression;
        }
    }
}
