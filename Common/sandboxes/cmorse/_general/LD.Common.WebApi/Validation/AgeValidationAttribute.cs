using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.Common.WebApi.Validation
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
            if (dateOfBirth.AddYears(MinimumAge) <= DateTime.Now)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessageString);
        }
    }
}
