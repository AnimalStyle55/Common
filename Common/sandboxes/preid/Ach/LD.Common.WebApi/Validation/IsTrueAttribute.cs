using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.Common.WebApi.Validation
{
    /// <summary>
    /// Validate True/False (checkbox)
    /// </summary>
    public class IsTrueAttribute : ValidationAttribute
    {
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

            if (value is bool
                && (bool)value == true)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage);
        }
    }
}
