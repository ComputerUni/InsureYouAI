using System.ComponentModel.DataAnnotations;

namespace InsureYouAI.Validations
{
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is bool b && b == true;
        }
    }
}
