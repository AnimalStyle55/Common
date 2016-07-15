using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.Common.Extensions
{
    /// <summary>
    /// Extensions for Enums
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Pulls the description of an Enum from the Description attribute, if it exists
        /// </summary>
        /// <param name="value"></param>
        /// <returns>the description of the Enum value</returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
