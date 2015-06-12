using System;

namespace LINDI.Core
{
    /// <summary>
    /// Defines an attribute that marks a method as a stub for a binding definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BindMethodAttribute : Attribute
    {
    }
}