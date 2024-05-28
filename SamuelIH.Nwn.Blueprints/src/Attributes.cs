using System;

namespace SamuelIH.Nwn.Blueprints
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NonOptionalInheritedAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InheritedAttribute : Attribute
    {
    }
}