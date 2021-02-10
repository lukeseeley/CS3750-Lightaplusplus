using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        public int MinimumAge { get; }

        public string GetErrorMessage() =>
            $"You must be at least {MinimumAge} years old.";

        public MinimumAgeAttribute(int minimumAge)
        {
            MinimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            var Birthday = (DateTime)value;
            
            // get how old they are
            DateTime age = Birthday.AddYears(MinimumAge);

            // check to see how old they are
            if (age > DateTime.Now)
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

    }
}
