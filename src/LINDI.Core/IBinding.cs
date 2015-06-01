namespace LINDI.Core
{
    /// <summary>
    /// Defines an interface that represents a binding from an interface to a specified type.
    /// </summary>
    /// <typeparam name="TInterface">The type that is being bound to another type.</typeparam>
    public interface IBinding<out TInterface>
    {
    }
}
