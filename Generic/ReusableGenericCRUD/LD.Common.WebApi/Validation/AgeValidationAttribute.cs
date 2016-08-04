using System;
using System.ComponentModel.DataAnnotations;

namespace Common.WebApi.Validation
{
    /// <summary>
    /// Validate an age value
    /// </summary>
    public class AgeValidationAttribute : ValidationAttribute
    {
        private int MinimumAge { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minimumAge">int years</param>
        public AgeValidationAttribute(int minimumAge)
        {
            MinimumAge = minimumAge;
        }

        /// <summary>
        /// Override IsValid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
                return ValidationResult.Success;

            DateTime dateOfBirth = (DateTime)value;

            return (dateOfBirth.AddYears(MinimumAge) <= DateTime.Now) ? ValidationResult.Success : new ValidationResult(ErrorMessageString); 
        }
    }
}
