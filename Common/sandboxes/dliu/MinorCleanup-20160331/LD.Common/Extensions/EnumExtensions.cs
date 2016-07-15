using CuttingEdge.Conditions;
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

        /// <summary>
        /// Converts to another enum by name (not by integral value)
        /// </summary>
        /// <param name="value"></param>
        /// <returns>an enum value of type T that has the same value as the one from value</returns>
        public static T ToEnum<T>(this Enum value) where T: struct, IConvertible
        {
            Condition.Requires("T must be an Enum").Evaluate(typeof(T).IsEnum);
            Condition.Requires(value, nameof(value)).IsNotNull();

            return value.ToString().ToEnum<T>();
        }

        /// <summary>
        /// Converts to another enum by name (not by integral value), supports nullable enums and value not matching
        /// </summary>
        /// <param name="value"></param>
        /// <returns>an enum value of type T that has the same value as the one from value, null if value was null or no match</returns>
        public static T? ToEnumOrNull<T>(this Enum value) where T: struct, IConvertible
        {
            Condition.Requires("T must be an Enum").Evaluate(typeof(T).IsEnum);

            T outVal;
            if (value != null && Enum.TryParse(value.ToString(), true, out outVal))
                return outVal;

            return null;
        }

        /// <summary>
        /// Converts to another enum by name (not by integral value), supports nullable enums and value not matching
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultVal"></param>
        /// <returns>an enum value of type T that has the same value as the one from value, defaultVal if there was no match or value was null</returns>
        public static T ToEnumOrDefault<T>(this Enum value, T defaultVal) where T: struct, IConvertible
        {
            return ToEnumOrNull<T>(value) ?? defaultVal;
        }
    }
}
