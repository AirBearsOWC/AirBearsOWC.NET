using System.ComponentModel.DataAnnotations;

namespace AirBears.Web.ViewModels
{
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }
    }
}
