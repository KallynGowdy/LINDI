using System;
using JetBrains.Annotations;

namespace LINDI.Core.Bindings
{
    /// <summary>
    /// Defines a class that represents an exception that is thrown when a binding does not resolve
    /// properly. Be sure to check the inner exception for details on what actually happened.
    /// </summary>
    public class BindingResolutionException : Exception
    {
        /// <summary>
        /// Gets the type of the <see cref="IBinding{TInterface}"/> that was attempted to be resolved.
        /// </summary>
        [NotNull]
        public Type BindingType { get; }

        /// <summary>
        /// Creates a new <see cref="BindingResolutionException"/> that represents
        /// an exception that was thrown when trying to resolve a binding.
        /// </summary>
        /// <param name="bindingType">The type of the <see cref="IBinding{TInterface}"/> object that was trying to resolve a value.</param>
        /// <param name="innerException">The exception that caused the resolution to fail.</param>
        public BindingResolutionException([NotNull] Type bindingType, Exception innerException) 
            : this($"Binding resolution failed for a binding of type {bindingType}. Check out the inner exception for more details.", bindingType, innerException)
        {
        }

        /// <summary>
        /// Creates a new <see cref="BindingResolutionException"/> that represents
        /// an exception that was thrown when trying to resolve a binding.
        /// </summary>
        /// <param name="message">The message that describes the exception.</param>
        /// <param name="bindingType">The type of the <see cref="IBinding{TInterface}"/> object that was trying to resolve a value.</param>
        /// <param name="innerException">The exception that caused the resolution to fail.</param>
        public BindingResolutionException(string message, [NotNull] Type bindingType, Exception innerException) 
            : base(message, innerException)
        {
            if (bindingType == null) throw new ArgumentNullException(nameof(bindingType));
            BindingType = bindingType;
        }
    }
}