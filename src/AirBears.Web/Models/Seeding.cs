using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Migrations;
using System.Linq;

namespace AirBears.Web.Models
{
    public static class Seeding
    {
        /// <summary>
        /// Returns true if all database migrations have been applied.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static void EnsureSeedData(this AppDbContext context)
        {
            if (context.AllMigrationsApplied())
            {
                context.SeedRoles();
                context.SeedStates();
                context.SeedTeeShirtSizes();
            }
        }

        private static void SeedStates(this AppDbContext context)
        {
            if (!context.States.Any())
            {
                context.States.AddRange(
                    new State { Name = "Alabama", Abbr = "AL" },
                    new State { Name = "Alaska", Abbr = "AK" },
                    new State { Name = "Arizona", Abbr = "AZ" },
                    new State { Name = "Arkansas", Abbr = "AR" },
                    new State { Name = "California", Abbr = "CA" },
                    new State { Name = "Colorado", Abbr = "CO" },
                    new State { Name = "Connecticut", Abbr = "CT" },
                    new State { Name = "Delaware", Abbr = "DE" },
                    new State { Name = "Florida", Abbr = "FL" },
                    new State { Name = "Georgia", Abbr = "GA" },
                    new State { Name = "Hawaii", Abbr = "HI" },
                    new State { Name = "Idaho", Abbr = "ID" },
                    new State { Name = "Illinois", Abbr = "IL" },
                    new State { Name = "Indiana", Abbr = "IN" },
                    new State { Name = "Iowa", Abbr = "IA" },
                    new State { Name = "Kansas", Abbr = "KS" },
                    new State { Name = "Kentucky", Abbr = "KY" },
                    new State { Name = "Louisiana", Abbr = "LA" },
                    new State { Name = "Maine", Abbr = "ME" },
                    new State { Name = "Maryland", Abbr = "MD" },
                    new State { Name = "Massachusetts", Abbr = "MA" },
                    new State { Name = "Michigan", Abbr = "MI" },
                    new State { Name = "Minnesota", Abbr = "MN" },
                    new State { Name = "Mississippi", Abbr = "MS" },
                    new State { Name = "Missouri", Abbr = "MO" },
                    new State { Name = "Montana", Abbr = "MT" },
                    new State { Name = "Nebraska", Abbr = "NE" },
                    new State { Name = "Nevada", Abbr = "NV" },
                    new State { Name = "New Hampshire", Abbr = "NH" },
                    new State { Name = "New Jersey", Abbr = "NJ" },
                    new State { Name = "New Mexico", Abbr = "NM" },
                    new State { Name = "New York", Abbr = "NY" },
                    new State { Name = "North Carolina", Abbr = "NC" },
                    new State { Name = "North Dakota", Abbr = "ND" },
                    new State { Name = "Ohio", Abbr = "OH" },
                    new State { Name = "Oklahoma", Abbr = "OK" },
                    new State { Name = "Oregon", Abbr = "OR" },
                    new State { Name = "Pennsylvania", Abbr = "PA" },
                    new State { Name = "Rhode Island", Abbr = "RI" },
                    new State { Name = "South Carolina", Abbr = "SC" },
                    new State { Name = "South Dakota", Abbr = "SD" },
                    new State { Name = "Tennessee", Abbr = "TN" },
                    new State { Name = "Texas", Abbr = "TX" },
                    new State { Name = "Utah", Abbr = "UT" },
                    new State { Name = "Vermont", Abbr = "VT" },
                    new State { Name = "Virginia", Abbr = "VA" },
                    new State { Name = "Washington", Abbr = "WA" },
                    new State { Name = "West Virginia", Abbr = "WV" },
                    new State { Name = "Wisconsin", Abbr = "WI" },
                    new State { Name = "Wyoming", Abbr = "WY" },
                    new State { Name = "American Samoa", Abbr = "AS" },
                    new State { Name = "District of Columbia", Abbr = "DC" },
                    new State { Name = "Federated States of Micronesia", Abbr = "FM" },
                    new State { Name = "Guam", Abbr = "GU" },
                    new State { Name = "Marshall Islands", Abbr = "MH" },
                    new State { Name = "Northern Mariana Islands", Abbr = "MP" },
                    new State { Name = "Palau", Abbr = "PW" },
                    new State { Name = "Puerto Rico", Abbr = "PR" },
                    new State { Name = "Virgin Islands", Abbr = "VI" }
                );

                context.SaveChanges();
            }
        }

        private static void SeedTeeShirtSizes(this AppDbContext context)
        {
            if (!context.TeeShirtSizes.Any())
            {
                context.TeeShirtSizes.AddRange(
                    new TeeShirtSize { Name = "S", SortOrder = 0 },
                    new TeeShirtSize { Name = "M", SortOrder = 1 },
                    new TeeShirtSize { Name = "L", SortOrder = 2 },
                    new TeeShirtSize { Name = "XL", SortOrder = 3 },
                    new TeeShirtSize { Name = "2XL", SortOrder = 4 },
                    new TeeShirtSize { Name = "3XL", SortOrder = 5 },
                    new TeeShirtSize { Name = "4XL", SortOrder = 6 },
                    new TeeShirtSize { Name = "5XL", SortOrder = 7 }
                );

                context.SaveChanges();
            }
        }

        private static void SeedRoles(this AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new IdentityRole { Id = "1", Name = Roles.Admin },
                    new IdentityRole { Id = "2", Name = Roles.Authority }
                );

                context.SaveChanges();
            }
        }
    }
}
