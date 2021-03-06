using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Lindi.Core.Bindings
{
    /// <summary>
    /// Defines a class that represents an exception that is thrown when a binding does not resolve
    /// properly. Be sure to check the inner exception for details on what actually happened.
    /// </summary>
    [Serializable]
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

        protected BindingResolutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            BindingType = (Type) info.GetValue(nameof(BindingType), typeof(Type));
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(BindingType), this.BindingType, typeof(Type));
        }
    }
}