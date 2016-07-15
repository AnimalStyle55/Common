using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LD.Common.WebApi.Validation
{
    /// <summary>
    /// Validate IP Address value
    /// </summary>
    public class IPAddressAttribute : ValidationAttribute
    {
        private const string _ErrorMessage = "Invalid IP address";

        /// <summary>
        /// Constructor
        /// </summary>
        public IPAddressAttribute()
        {
            ErrorMessage = _ErrorMessage;
        }

        /// <summary>
        /// Override IsValid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            IPAddress dummy; // Don't care about the parsed result, just that it's valid
            if (value is string
                && IPAddress.TryParse((string)value, out dummy))
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage);
        }
    }
}
