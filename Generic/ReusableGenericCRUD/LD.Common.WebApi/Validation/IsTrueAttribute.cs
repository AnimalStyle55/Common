using System.ComponentModel.DataAnnotations;

namespace Common.WebApi.Validation
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

            return (value is bool && (bool)value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
