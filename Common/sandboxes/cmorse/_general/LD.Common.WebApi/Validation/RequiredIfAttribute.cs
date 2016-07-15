using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.Common.WebApi.Validation
{
    /// <summary>
    /// Validate required field exists
    /// </summary>
    public class RequiredIfAttribute : RequiredAttribute
    {
        private string PropertyName { get; set; }
        private object DesiredValue { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RequiredIfAttribute(String propertyName, Object desiredValue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredValue;
        }

        /// <summary>
        /// Override IsValid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;
            Type type = instance.GetType();

            object propertyValue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (object.Equals(propertyValue, DesiredValue))
                return base.IsValid(value, context);

            return ValidationResult.Success;
        }
    }
}
