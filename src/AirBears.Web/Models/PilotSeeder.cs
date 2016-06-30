using AirBears.Web.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirBears.Web.Models
{
    public static class PilotSeeder
    {
        public static void InvitePilots(AppDbContext context, IMailer mailer)
        {
            var userStore = new UserStore<User>(context);
            var hasher = new PasswordHasher<User>();
            var usernames = UsersToInvite; //new[] { "tomfaltesek@gmail.com", "tom.faltesek@gmail.com", "tomfaltese.k@gmail.com", "tomfalte.sek@gmail.com" };
            var users = context.Users.Where(u => usernames.Contains(u.UserName) && !u.LastLoginDate.HasValue && !u.LastPasswordChangeDate.HasValue).ToList();

            foreach (var user in users)
            {
                var password = GenerateWelcomePassword();
                var body = $"Hello { user.FirstName }!<br /><br />The newest version of our web application was recently launched. You've been assigned a temporary password (below) to access your new account. "
                    + "You're encouraged to log in to your new account and change your password to something you can easily remember.<br /><br />"
                    + $"<b>Username:</b> { user.UserName }<br />"
                    + $"<b>Password:</b> { password }"
                    + "<br /><br />Visit <a href='https://www.airbears.org' target='_blank'>www.airbears.org</a> to access your account.<br /><br />"
                    + "Your new account will grant you access to our pilot locator and allow you to edit your pilot profile. There are many plans for future profile and application additions. "
                    + "We'll send you an update when significant features are added. In the meantime, we appreciate any feedback you may have.<br /><br />"
                    + "Thank you,<br />Air Bears Team";

                mailer.SendAsync(user.Email, "Welcome to Air Bears", body, true, false).Wait();

                user.PasswordHash = hasher.HashPassword(user, password);
                user.LastPasswordChangeDate = DateTime.UtcNow;

                userStore.UpdateAsync(user).Wait();
            }
        }

        private static string GenerateWelcomePassword()
        {
            var options = "0123456789abcdefghjkmnopqrstuvwxyz"; // excluded i and l to avoid confusion.
            var rand = new Random();
            var result = "Welcome-";

            for (var i = 0; i < 6; i++)
            {
                var next = rand.Next(0, options.Length - 1);
                result += options[next];
            }

            return result + "1";
        }

        private static List<string> UsersToInvite
        {
            get
            {
                return new List<string>()
                {
                    "terrill503@yahoo.com", "christman.gordon@gmail.com", "kjedick@earthlink.net"
                };
            }
        }

        public static void SeedPilots(AppDbContext context)
        {
            var userStore = new UserStore<User>(context);
            var hasher = new PasswordHasher<User>();
            var geocodeService = new GeocodeService();
            var states = context.States.ToList();
            var shirts = context.TeeShirtSizes.ToList();

            foreach (var user in PilotsToBeSeeded)
            {
                if (!context.Users.Any(u => u.UserName.Equals(user.UserName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var newUser = MapProperties(user, states, shirts, hasher);
                    UpdatePilotCoordinates(newUser, user.State, geocodeService);

                    userStore.CreateAsync(newUser).Wait();
                }
            }
        }

        private static User MapProperties(MigratedUser user, IEnumerable<State> states, IEnumerable<TeeShirtSize> shirts, PasswordHasher<User> hasher)
        {
            var newUser = new User();

            newUser.UserName = user.UserName;
            newUser.FirstName = user.FirstName;
            newUser.LastName = user.LastName;
            newUser.PhoneNumber = string.IsNullOrWhiteSpace(user.Phone) ? null : user.Phone;

            newUser.Street1 = string.IsNullOrWhiteSpace(user.Street1) ? null : user.Street1;
            newUser.Street2 = string.IsNullOrWhiteSpace(user.Street2) ? null : user.Street2;
            newUser.City = string.IsNullOrWhiteSpace(user.City) ? null : user.City;
            newUser.Zip = string.IsNullOrWhiteSpace(user.Zip) ? null : user.Zip;

            newUser.HasInternationalAddress = user.HasInternationalAddress;
            newUser.AddressLine1 = string.IsNullOrWhiteSpace(user.AddressLine1) ? null : user.AddressLine1;
            newUser.AddressLine2 = string.IsNullOrWhiteSpace(user.AddressLine2) ? null : user.AddressLine2;
            newUser.AddressLine3 = string.IsNullOrWhiteSpace(user.AddressLine3) ? null : user.AddressLine3;
            newUser.AddressLine4 = string.IsNullOrWhiteSpace(user.AddressLine4) ? null : user.AddressLine4;

            newUser.DateRegistered = DateTime.Now;
            newUser.Email = user.UserName;
            newUser.NormalizedEmail = newUser.Email.ToUpper();
            newUser.NormalizedUserName = newUser.UserName.ToUpper();
            newUser.SecurityStamp = Guid.NewGuid().ToString();
            newUser.HasAgreedToTerms = true;
            newUser.AllowsPilotSearch = true;
            newUser.SubscribesToUpdates = true;
            newUser.TeeShirtMailedDate = DateTime.Now;
            newUser.LockoutEnabled = true;
            newUser.DateRegistered = DateTime.Now;
            newUser.PasswordHash = hasher.HashPassword(newUser, "temp-password@123!");

            // attempt to map state.
            var state = states.Where(s => s.Abbr.ToLower() == user.State.ToLower()).FirstOrDefault();
            if (state != null) { newUser.StateId = state.Id; }

            // attempt to map tee-shirt size.
            var shirt = shirts.Where(s => s.Name.ToLower() == user.TeeShirtSize.ToLower()).FirstOrDefault();
            if (shirt != null) { newUser.TeeShirtSizeId = shirt.Id; }

            return newUser;
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

        private static List<MigratedUser> PilotsToBeSeeded = new List<MigratedUser>
        {
            new MigratedUser { UserName = "terrill503@yahoo.com", FirstName = "William", LastName = "Terrill", Phone="503-665-3561", Street1 = "2516 SW Brixton Dr.", Street2 ="", City = "Gresham", State = "OR", Zip="97080", TeeShirtSize = "2XL", AddressLine1 = "", AddressLine2 =  "", AddressLine3 =  "", AddressLine4 =  "", HasInternationalAddress =  false },
            new MigratedUser { UserName = "christman.gordon@gmail.com", FirstName = "Chris", LastName = "Gordon", Phone="502-741-4352", Street1 = "2917 Cambridge Rd.", Street2 ="", City = "Louisville", State = "KY", Zip="40220", TeeShirtSize = "2XL", AddressLine1 = "", AddressLine2 =  "", AddressLine3 =  "", AddressLine4 =  "", HasInternationalAddress =  false },
            new MigratedUser { UserName = "kjedick@earthlink.net", FirstName = "Kenneth", LastName = "Edick", Phone="", Street1 = "27665 Mucho Grande View", Street2 ="", City = "Calhan", State = "CO", Zip="80808", TeeShirtSize = "XL", AddressLine1 = "", AddressLine2 =  "", AddressLine3 =  "", AddressLine4 =  "", HasInternationalAddress =  false }
        };
    }

    public class MigratedUser
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string TeeShirtSize { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public bool HasInternationalAddress { get; set; }
    }
}
