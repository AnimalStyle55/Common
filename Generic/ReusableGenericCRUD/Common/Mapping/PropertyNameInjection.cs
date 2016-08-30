using Common.Logging;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Mapping
{
    /// <summary>
    /// A custom ValueInjecter injection which will attempt to inject
    /// properties with the same name. Will perform string to enum conversion
    /// as well as deep injection of complex objects, arrays, and ICollections.
    /// 
    /// </summary>
    /// <example>
    /// Manual Injection
    /// <code>
    /// Bar bar = new Bar();
    /// bar.InjectFrom&lt;PropertyNameInjection&gt;(foo);
    /// </code>
    /// 
    /// Setting up the default injection for the Mapper static class
    /// <code>
    /// StaticValueInjecter.DefaultInjection = new PropertyNameInjection();
    /// Foo foo = new Foo() { };
    /// Mapper.Map&lt;Bar&gt;(foo);
    /// </code>
    /// </example>
    public class PropertyNameInjection : LoopInjection
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyNameInjection()
        {
        }

        /// <summary>
        /// Returns whether or not the source and target types are mappable.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected override bool MatchTypes(Type sourceType, Type targetType)
        {
            sourceType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (CanEnumInject(sourceType, targetType)
                || CanArrayInject(sourceType, targetType)
                || CanIListInject(sourceType, targetType)
                || CanDeepInject(sourceType, targetType))
                return true;

            return sourceType == targetType;
        }

        /// <summary>
        /// Set value of the target property using the source property mapping.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="sp"></param>
        /// <param name="tp"></param>
        protected override void SetValue(object source, object target, PropertyInfo sp, PropertyInfo tp)
        {
            try
            {
                tp.SetValue(target, GetTargetValue(source, sp, tp), null);
            }
            catch(Exception ex)
            {
                log.Warn($"Could not set value for property {tp.Name} ({tp.PropertyType.Name}) in {target.GetType().FullName}", ex);
            }
        }

        private object GetTargetValue(object source, PropertyInfo sp, PropertyInfo tp)
        {
            var sourceType = Nullable.GetUnderlyingType(sp.PropertyType) ?? sp.PropertyType;
            var targetType = Nullable.GetUnderlyingType(tp.PropertyType) ?? tp.PropertyType;

            object sourceVal = sp.GetValue(source, null);
            object targetVal;

            // Map from source to target

            if (sourceVal == null)
                targetVal = null;
            else if (targetType.IsEnum)
                targetVal = MapEnum(sourceVal, sourceType, targetType);
            else if (sourceType.IsEnum)
                targetVal = sourceVal.ToString();
            else if (targetType.IsArray)
                targetVal = MapArray(sourceVal, sourceType, targetType);
            else if (IsTypeIList(targetType))
                targetVal = MapIList(sourceVal, sourceType, targetType);
            else if(IsTypeDeepInjectable(targetType))
                targetVal = MapComplexObject(sourceVal, sourceType, targetType);
            else
                targetVal = sourceVal;

            return targetVal;
        }

        //
        // Mapping logic
        //

        private object MapEnum(object sourceVal, Type sourceType, Type targetType)
        {
            if (!Enum.IsDefined(targetType, sourceVal.ToString()))
                return null;

            return Enum.Parse(targetType, sourceVal.ToString());
        }

        private object MapArray(object sourceVal, Type sourceType, Type targetType)
        {
            var sourceArray = sourceVal as Array;
            var targetArray = Array.CreateInstance(targetType.GetElementType(), sourceArray.Length);

            var targetItemType = targetType.GetElementType();
            for (int index = 0; index < sourceArray.Length; index++)
            {
                var sourceItem = sourceArray.GetValue(index);
                var targetItem =
                    (targetItemType == typeof(string) || targetItemType.IsValueType) ?
                        sourceItem :
                        Activator.CreateInstance(targetItemType).InjectFrom<PropertyNameInjection>(sourceItem);
                targetArray.SetValue(targetItem, index);
            }

            return targetArray;
        }

        private object MapIList(object sourceVal, Type sourceType, Type targetType)
        {
            var sourceCollection = sourceVal as IEnumerable;
            var targetCollection = Activator.CreateInstance(targetType);

            var targetItemType = targetType.GetGenericArguments()[0];
            var addMethod = targetType.GetMethod("Add");
            foreach (var sourceItem in sourceCollection)
            {
                var targetItem =
                    (targetItemType == typeof(string) || targetItemType.IsValueType) ?
                        sourceItem :
                        Activator.CreateInstance(targetItemType).InjectFrom<PropertyNameInjection>(sourceItem);
                addMethod.Invoke(targetCollection, new[] { targetItem });
            }

            return targetCollection;
        }

        private object MapComplexObject(object sourceVal, Type sourceType, Type targetType)
        {
            return Activator.CreateInstance(targetType).InjectFrom<PropertyNameInjection>(sourceVal);
        }

        //
        // Mapping type matching
        //

        private bool CanEnumInject(Type sourceType, Type targetType)
        {
            return (sourceType.IsEnum || sourceType == typeof(string))
                && (targetType.IsEnum || targetType == typeof(string));
        }

        private bool CanArrayInject(Type sourceType, Type targetType)
        {
            if (!sourceType.IsArray
                || !targetType.IsArray)
                return false;

            var sourceItemType = sourceType.GetElementType();
            var targetItemType = targetType.GetElementType();

            return MatchTypes(sourceItemType, targetItemType);
        }

        private bool CanIListInject(Type sourceType, Type targetType)
        {
            if (!IsTypeIList(sourceType)
                || !IsTypeIList(targetType))
                return false;

            var sourceItemType = sourceType.GetGenericArguments()[0];
            var targetItemType = targetType.GetGenericArguments()[0];

            return MatchTypes(sourceItemType, targetItemType);
        }

        private bool CanDeepInject(Type sourceType, Type targetType)
        {
            return IsTypeDeepInjectable(sourceType)
                && IsTypeDeepInjectable(targetType);
        }

        //
        // Type inspectors
        //

        private bool IsTypeIList(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition().GetInterfaces()
                    .Any(x =>
                    x == typeof(IList)
                    || (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>)));
        }

        private bool IsTypeDeepInjectable(Type type)
        {
            return !(type.IsValueType || type == typeof(string))
                && !type.IsArray
                && !type.IsGenericType;
        }
    }
}
