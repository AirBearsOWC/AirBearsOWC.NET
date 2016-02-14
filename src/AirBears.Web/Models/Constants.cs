namespace AirBears.Web.Models
{
    public static class Roles
    {
        public const string Authority = "Authority";
        public const string Admin = "Admin";
        public const string Admin_And_Authority = Admin + "," + Authority;
    }

    public static class AuthPolicies
    {
        public const string Bearer = "Bearer";
    }
}
