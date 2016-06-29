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
                    "geoff.simmons@matcotools.com","steven@quickdry.net","waitnsea@hotmail.com","adam@aeroworksproductions.com","athapapuskowbrooks@gmail.com","raetown16@yahoo.com",
                    "hugob33@gmail.com","folepi2@chartermi.net","flyboone@yahoo.com","chriscote8686@gmail.com","scoughlin711@live.com","creech883@hotmail.com","bob@otcmma.com",
                    "mike.eisen@gmail.com","marc.espina@gmail.com","fitzdog5574@gmail.com","bgauweiler@gmail.com","paulsgreen72@gmail.com","dharbold@gmail.com","thesherm4@gmail.com",
                    "rxhenn@gmail.com","steve@icu2collectibles.com","taylorireton@gmail.com","ijohansenjr@gmail.net","paulj09@hotmail.com","sebastian.klempin@protonmail.ch",
                    "jrkorth@aol.com","dimi_kapa@hotmail.com","greensap.kl@gmail.com","awesomestuntguy@gmail.com","keithluneau@gmail.com","fm_machado@hotmail.com",
                    "simonmcdonald1@hotmail.co.uk","paul_montgomery2001@yahoo.com","ryan.moreau77@gmail.com","bshocky101@gmail.com","pontzers@gmail.com","chris.prodanov@gmail.com",
                    "brent@twinbear.com","reffnersgeckos@gmail.com","danrhyoo@gmail.com","malikroberson@gmail.com","brettsaunders1980@gmail.com","rdseberg@gmail.com","mshandrow@live.com",
                    "rsoko.rs@gmail.com","sales@rcaerialplatforms.com","jstevens9416@gmail.com","sharpimage@photographer.net","puddinpaulie@aol.com","takeflightaviationct@gmail.com",
                    "thom7215@gmail.com","tedven@comcast.net","planet892iv@earthlink.net","puddinpaulie@aol.com","melbear@optonline.net","diff112@verizon.net","chrisdudley1979@yahoo.com",
                    "jenos83@gmail.com","tom@toms-workshop.com","derrickh24@hotmail.com","billbo911@gmail.com","tminnick@gmail.com","overholtjl@gmail.com","ryanquigg@yahoo.ca",
                    "maingear81@gmail.com","bryton2016@gmail.com","yardrestorations@gmail.com","dino300zx@gmail.com","jim@jimbrammer.com","achervitz@yahoo.com","seandcoffey@yahoo.com",
                    "anthony_cuington@yahoo.com","mhonster@msn.com","zfotoguy71@yahoo.com","jonathanroberts@yahoo.com","tscott@cityofandersonsc.com","ray-pecsk8rfreak@hotmail.com",
                    "acevesandres@yahoo.com","clbrumley@hotmail.com","jcieri227@gmail.com","colby.chuck@gmail.com","agduvall@gmail.com","jossefg@gmail.com","tony@hicksonfamily.net",
                    "eho@dpapyrus.com","joshuatoddjones@gmail.com","vlaluz@gmail.com","parsonsh@asme.org","magicman9493@gmail.com","binarynut@comcast.net","mike@tiwc.demon.co.uk",
                    "kime@otcmma.com","jacobmervine@gmail.com","zpackingham@gmail.com","pskk1@earthlink.net","pyrotec41@gmail.com","mandar.shendge@gmail.com","donstang165@gmail.com",
                    "dave@birdseye-ap.com","nc4rg@yandex.com","jalindstrom@comcast.net","johnerusscher@gmail.com","mr.rich.baker@gmail.com","rssb_baker@yahoo.com","akcobra@cox.net",
                    "jakebarritt929@yahoo.com","sbellamy84@gmail.com","carpy@q.com","frank.debros@gmail.com","bryan@thedukes.org","rjevans@tethys-aeronautica.com","Fabiojesepi@gmail.com",
                    "milesgary@gmail.com","prsark9@gmail.com","crash.hancock@gmail.com","mike@qavmedia.com","thrytsay@gmail.com","landerjorens@mac.com","kallend@iit.edu",
                    "john.d.kostis@gmail.com","ryan.labarre@gmail.com","andrew.mcclenaghan@gmail.com","phil@concretephil.com","spencerrawnsley@gmail.com","militaryblaze@yahoo.com",
                    "mschnoblen@gmail.com","msoto@photosbysoto.com","schlank99@verizon.net","rstiles@gmail.com","kdsfloz@gmail.com","xcessive@gmail.com","ahawk89@gmail.com",
                    "vivsoft@gmail.com","tw@b-a-l-u.edu","kbrianward@gmail.com","mawright@hawaii.edu","lorenjz@gmail.com","thenorthface25@yahoo.com","prpsjeans@gmail.com",
                    "aj@dodleston.com","joshua_slager@yahoo.com","mark@markcosy.com","bztchr1@gmail.com","ajh72010@gmail.com","volusiafpv@gmail.com","toddw@1drc.com",
                    "wattsduane@hotmail.com","abeyoung1@gmail.com","mawright@hawaii.edu","olly.barry@outlook.com","juiceguy@juiceguy.com","pipezsantiago983@gmail.com",
                    "Altairvideography@gmail.com","renedgotiear@hotmail.com","ryanscivic@gmail.com","xavierz84@comcast.net","levelox@aol.com","contact@nicolashenri.ch",
                    "joshua@twenzel.com","brburke2002@yahoo.com","jgish63@gmail.com","charlieboto@gmail.com","jmkirsch@gmail.com","josh@birdsiproductions.com","dlangdon@gmail.com",
                    "info@proflyaerial.com","green_silvia@hotmail.com","spectre909@gmail.com","blakewood33@gmail.com","carkba@gmail.com","tom@tompaulus.com","sshepard@tighebond.com",
                    "hotcarp12@hotmail.com","kbates@mihomes.com","crouchcrew@comcast.net","ted@kjstar.com","mikeayoung@live.ca","davidmcgroarty@gmail.com","gregory.moultrie@gmail.com",
                    "jrhautzinger@gmail.com","tony@tosuma.com","xcessive@gmail.com","dkraft@whatsinthere.com","EricOlson06@gmail.com","Andrereinerttkd@hotm​ail.com","eliotgillum@hotmail.com",
                    "DTEngineeringinc@gmail.com","justinrutkauskas@yahoo.com","drotte63@gmail.com","taylor.ratliff@gmail.com","prointellect92@gmail.com","usrbingeek@gmail.com",
                    "john@elevated-solutions.net","birch.b.hansen@gmail.com","vandenking@gmail.com","allelectric50@yahoo.com","euan@phoenix-uas.com","m_schara@yahoo.com","flcc77@gmail.com",
                    "kathy@weflysky.com","dansmithpdx@gmail.com","gstowers@frontier.com","kodiak181@gmail.com","jeramiemorris@hotmail.com","durensdrones@gmail.com","blackoutconcepts@gmail.com",
                    "timbaker@tcbconsulting.com","jessikafarrar@gmail.com","pav.ioann@gmail.com","smitty32277@comcast.net","wes.hayward@sbcglobal.net","holt.abn@gmail.com",
                    "loucanada2001@yahoo.com","kevinsilva@sympatico.ca","rihnodoc@optonline.net","bruce@SightFlight.com","mario@fabrizios.com","joshmay1@comcast.net","droneflyermn@hotmail.com",
                    "russglider@bellsouth.net","joeturnus@gmail.com","info@northernlights3d.com","coryshubert@gmail.com","svc227@hotmail.com","robertz@flightventuresltd.com",
                    "teneightvideo@gmail.com","bdlj.wf@gmail.com","scoobyvroom@charter.net","dmyers777@mac.com","mwheeler2013@gmail.com","alan@akb.net","zaplocked@gmail.com",
                    "joshua.kennedy3@gmail.com","jon.rera1@yahoo.com","brent@gorillaprod.com","melyash@surewest.net","daninselman@hotmail.com","chickenh4wk@gmail.com","benluman@hotmail.com",
                    "robyn@keyconcepts.com","aug.46@hotmail.com","14.medic@gmail.com","robertashaw@live.com","yazbak@chartermi.net","leg@leadingedgegliders.com","darrengoodbar@gmail.com",
                    "fasted2629@yahoo.com","lawson.conner@gmail.com","brandon@id3studies.com","iandunlop@bell.net","matt.dunlop@mattcave.cu","charles.durham@seisan.com","snfidler@gmail.com",
                    "andrew.grant.foster@gmail.com","mikefuller@berryplastics.com","bgfireguy92@gmail.com","info@stoneblueairlines.com","jer_walt_p99@yahoo.com","dash@whisper-wireless.com",
                    "nholt48@aol.com","chad@stonekap.com","joshuatlatham@gmail.com","mnl5099@gmail.com","archerx10@charter.net","computerbob99@gmail.com","matt@stoneblueairlines.com",
                    "roodster2000@yahoo.com","ait.photo@yahoo.com","cory.samuell@gmail.com","monty134jam@gmail.com","diesellogs@aol.com","sonnmakerj@gmail.com","tjs@vt.edu.com",
                    "military.1995@gmail.com","rich.bellamy@gmail.com","Joe.Birchhill@ecolab.com","mbohl@tx-mb.org","gjbryl@sbcglobal.net","stradawhovious@yahoo.com","jclines41@gmail.com",
                    "drewculver@gmail.com","colt@coltfreeman.com","mike@buttonwillow.com","mgoodwin89@yahoo.com","thomassharrisjr@gmail.com","casedog21@gmail.com","greghoudyshell@gmail.com",
                    "jeleniewskic@yahoo.com","skocsis@windstream.net","jack@replity.com","j.moser1974@gmail.com","d.ryan.papp@gmail.com","jakrabit@usa.net","allen@aerorecon.com",
                    "eastcoastdrone@gmail.com","g.gsimpson@gmail.com","dbwennberg@visi.com","kennbyee25@gmail.com","tony933@yahoo.com","scottyz333@gmail.com","emaildougb@gmail.com",
                    "steen.greg@gmail.com","doug_hetzler@comcast.net","mebillica@gmail.com","davidyork174@yahoo.com","msh312b@gmail.com","uav@ispysecuritysystems.com","justin@texasbyair.com",
                    "dctisthebest@gmail.com","domcperez@gmail.com"
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
