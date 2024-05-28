using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;

namespace SamuelIH.Nwn.Blueprints
{
    public abstract class Blueprint
    {
        private static readonly Dictionary<Type, CachedReflectionData> cachedReflectionData = new();

        internal string name = "";
        internal string baseName = "";

        [YamlMember(Alias = "parent")]
        public virtual string? _Parent { get; set; }

        internal virtual Blueprint? Parent { get; set; }

        [YamlMember(Alias = "blueprintType")]
        public string BlueprintType { get; set; } = "";
        
        [YamlIgnore]
        public string FilePath { get; set; } = "";

        private CachedReflectionData GetCachedReflectionData()
        {
            var type = GetType();

            if (!cachedReflectionData.ContainsKey(type))
            {
                var nonOptionalProps = type.GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(NonOptionalInheritedAttribute)));

                var optionalProps = type.GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(InheritedAttribute)));

                cachedReflectionData[type] =
                    new CachedReflectionData(nonOptionalProps.ToArray(), optionalProps.ToArray());
            }

            return cachedReflectionData[type];
        }

        /// <summary>
        /// Find the closest parent up the chain that has a non-null value for the given property, or null if none found.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal (Blueprint parent, object value)? ResolveParent(PropertyInfo prop)
        {
            var parent = Parent;
            while (parent is not null)
            {
                if (prop.GetValue(parent) is object value)
                {
                    return (parent, value);
                }

                parent = parent?.Parent;
            }

            return null;
        }
        
        internal object? ResolveProperty(PropertyInfo prop) => ResolveParent(prop)?.value;
        
        internal void ResolveOverridableConstructIfNeeded(PropertyInfo prop)
        {
            if (prop.GetValue(this) is not IOverridableConstruct construct) return;
            if (construct.IsResolved) return;

            if (ResolveParent(prop) is not (Blueprint parent, object parentValue))
            {
                construct.ResolveFromParent(null);
                return;
            }
            
            parent.ResolveOverridableConstructIfNeeded(prop);
            construct.ResolveFromParent(parentValue);
        }

        /// <summary>
        ///     Attempts to resolve all the properties of this blueprint.
        ///     Returns false if any of the properties are not resolved.
        /// </summary>
        /// <returns></returns>
        internal virtual bool ResolveProperties()
        {
            var refData = GetCachedReflectionData();

            foreach (var prop in refData.NonOptionalProps)
            {
                if (prop.GetValue(this) is not null)
                {
                    ResolveOverridableConstructIfNeeded(prop);
                    continue;
                }

                if (ResolveProperty(prop) is not object value) return false;
                prop.SetValue(this, value);
            }

            foreach (var prop in refData.OptionalProps)
            {
                if (prop.GetValue(this) is not null)
                {
                    ResolveOverridableConstructIfNeeded(prop);
                    continue;
                }
                
                if (ResolveProperty(prop) is not object value) continue;
                prop.SetValue(this, value);
            }

            return true;
        }

        private class CachedReflectionData
        {
            public CachedReflectionData(PropertyInfo[] nonOptionalProps, PropertyInfo[] optionalProps)
            {
                NonOptionalProps = nonOptionalProps;
                OptionalProps = optionalProps;
            }

            public PropertyInfo[] NonOptionalProps { get; }
            public PropertyInfo[] OptionalProps { get; }
        }
    }
}