using AirBears.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
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

        public static void EnsureSeedData(this AppDbContext context, bool isDevEnv = false)
        {
            if (context.AllMigrationsApplied())
            {
                context.SeedRoles();
                context.SeedStates();
                context.SeedTeeShirtSizes();
                context.SeedPayloads();
                context.SeedFlightTimes();

                if (isDevEnv)
                {
                    context.SeedUsers();
                }

                //PilotSeeder.SeedPilots(context);
            }
        }

        //public static void InviteMigratedUsers(this AppDbContext context, IMailer mailer)
        //{
        //    if (context.AllMigrationsApplied())
        //    {
        //        PilotSeeder.InvitePilots(context, mailer);
        //    }
        //}

        private static void SeedUsers(this AppDbContext context)
        {            
            var userStore = new UserStore<User>(context);
            var hasher = new PasswordHasher<User>();
            var geocodeService = new GeocodeService();
            var mnState = context.States.Where(s => s.Abbr.ToLower() == "mn").FirstOrDefault();
            var largeShirt = context.TeeShirtSizes.Where(s => s.Name.ToLower() == "l").FirstOrDefault();

            var adminEmail = "admin@airbears.org";
            if (!context.Users.Any(u => u.Email == adminEmail))
            {
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "Jones",
                    Street1 = "123 Hope Ave.",
                    Bio = "This is an admin development user.",
                    City = "St. Paul",
                    StateId = mnState.Id,
                    Zip = "55105",
                    PhoneNumber = "651-999-9999",
                    DateRegistered = DateTime.UtcNow,
                    TeeShirtMailedDate = DateTime.UtcNow,
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpper(),
                    TeeShirtSizeId = largeShirt.Id,
                    AllowsPilotSearch = true,
                    HasAgreedToTerms = true,
                    SubscribesToUpdates = true,
                    HasInternationalAddress = false
                };
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "password");

                UpdatePilotCoordinates(adminUser, mnState.Abbr, geocodeService);
                userStore.CreateAsync(adminUser).Wait();
                //adminUser = context.Users.First(u => u.Email == adminEmail);
                userStore.AddToRoleAsync(adminUser, Roles.Admin).Wait();
            }

            var authorityEmail = "authority@airbears.org";
            if (!context.Users.Any(u => u.Email == authorityEmail))
            {
                var authorityUser = new User
                {
                    FirstName = "Authority",
                    LastName = "Smith",
                    Street1 = "222 Main St.",
                    Bio = "This is an approved authority development user.",
                    City = "Minneapolis",
                    StateId = mnState.Id,
                    Zip = "55112",
                    PhoneNumber = "651-888-8888",
                    DateRegistered = DateTime.UtcNow,
                    TeeShirtMailedDate = DateTime.UtcNow,
                    Email = authorityEmail,
                    NormalizedEmail = authorityEmail.ToUpper(),
                    UserName = authorityEmail,
                    NormalizedUserName = authorityEmail.ToUpper(),
                    TeeShirtSizeId = largeShirt.Id,
                    AllowsPilotSearch = true,
                    HasAgreedToTerms = true,
                    SubscribesToUpdates = true,
                    HasInternationalAddress = false,
                    IsAuthorityAccount = true
                };
                authorityUser.PasswordHash = hasher.HashPassword(authorityUser, "password");              

                UpdatePilotCoordinates(authorityUser, mnState.Abbr, geocodeService);
                userStore.CreateAsync(authorityUser).Wait();
                //authorityUser = context.Users.First(u => u.Email == adminEmail);
                userStore.AddToRoleAsync(authorityUser, Roles.Authority).Wait(); // Autority role signifies a confirmed authority account.
            }

            var unapprovedAuthorityEmail = "unapproved-authority@airbears.org";
            if (!context.Users.Any(u => u.Email == unapprovedAuthorityEmail))
            {
                var unapprovedAuthority = new User
                {
                    FirstName = "UnapprovedAuthority",
                    LastName = "Johnson",
                    Street1 = "515 1st Ave.",
                    Bio = "This is an unapproved authority development user.",
                    City = "St. Paul",
                    StateId = mnState.Id,
                    Zip = "55112",
                    PhoneNumber = "651-888-9999",
                    DateRegistered = DateTime.UtcNow,
                    TeeShirtMailedDate = DateTime.UtcNow,
                    Email = unapprovedAuthorityEmail,
                    NormalizedEmail = unapprovedAuthorityEmail.ToUpper(),
                    UserName = unapprovedAuthorityEmail,
                    NormalizedUserName = unapprovedAuthorityEmail.ToUpper(),
                    TeeShirtSizeId = largeShirt.Id,
                    AllowsPilotSearch = true,
                    HasAgreedToTerms = true,
                    SubscribesToUpdates = true,
                    HasInternationalAddress = false,
                    IsAuthorityAccount = true
                };
                unapprovedAuthority.PasswordHash = hasher.HashPassword(unapprovedAuthority, "password");

                UpdatePilotCoordinates(unapprovedAuthority, mnState.Abbr, geocodeService);
                userStore.CreateAsync(unapprovedAuthority).Wait();
            }
        }

        private static void SeedPayloads(this AppDbContext context)
        {
            if (!context.Payloads.Any())
            {
                context.Payloads.AddRange(
                    new Payload { Name = "None", SortOrder = 0 },
                    new Payload { Name = "1 Pound", SortOrder = 1 },
                    new Payload { Name = "2 Pounds", SortOrder = 2 },
                    new Payload { Name = "3 Pounds", SortOrder = 3 },
                    new Payload { Name = "4 Pounds", SortOrder = 4 },
                    new Payload { Name = "5+ Pounds", SortOrder = 5 }
                );

                context.SaveChanges();
            }
        }

        private static void SeedFlightTimes(this AppDbContext context)
        {
            if (!context.FlightTimes.Any())
            {
                context.FlightTimes.AddRange(
                    new FlightTime { Name = "1-2 Minutes", SortOrder = 0 },
                    new FlightTime { Name = "2-5 Minutes", SortOrder = 1 },
                    new FlightTime { Name = "5-10 Minutes", SortOrder = 2 },
                    new FlightTime { Name = "10+ Minutes", SortOrder = 3 }
                );

                context.SaveChanges();
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

        private static void UpdatePilotCoordinates(User pilot, string state, GeocodeService geocodeService)
        {
            var coords = geocodeService.GetCoordsForAddress(pilot.GetAddress(state)).Result;

            if (coords.Status != GeocodeResponseStatus.OK)
            {
                return;
            }

            pilot.Longitude = coords.Longitude;
            pilot.Latitude = coords.Latitude;
            pilot.GeocodeAddress = coords.GeocodeAddress;
        }

    }
}
