using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace AirBears.Web.Models
{
    public class AppDbContext : IdentityDbContext<User>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<TeeShirtSize> TeeShirtSizes { get; set; }

        //public DbSet<Country> Countries { get; set; }

        public DbSet<State> States { get; set; }
    }
}
