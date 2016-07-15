using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.Common.WebApi.Validation
{
    /// <summary>
    /// Validate valid US State
    /// </summary>
    public class USStateAttribute : ValidationAttribute
    {
        private static readonly ISet<string> ValidStates = new HashSet<string>
        {
            "AL",
            "AK",
            "AZ",
            "AR",
            "CA",
            "CO",
            "CT",
            "DE",
            "DC",
            "FL",
            "GA",
            "HI",
            "ID",
            "IL",
            "IN",
            "IA",
            "KS",
            "KY",
            "LA",
            "ME",
            "MD",
            "MA",
            "MI",
            "MN",
            "MS",
            "MO",
            "MT",
            "NE",
            "NV",
            "NH",
            "NJ",
            "NM",
            "NY",
            "NC",
            "ND",
            "OH",
            "OK",
            "OR",
            "PA",
            "RI",
            "SC",
            "SD",
            "TN",
            "TX",
            "UT",
            "VT",
            "VA",
            "WA",
            "WV",
            "WI",
            "WY"
        };

        private const string _ErrorMessage = "Invalid state";

        /// <summary>
        /// Constructor
        /// </summary>
        public USStateAttribute() : base()
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

            if (value is string
                && ValidStates.Contains(((string)value).ToUpper()))
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage);
        }
    }
}
