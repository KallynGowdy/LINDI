# Binding
This document represents the description of the API for the `IBinding<TInterface>` interface.

Bindings are used to represent relationships between an interface (or super) type and a
corresponding derived type. Bindings represent this relationship in the context of
providing a value. In the case of dependency injection, bindings act as resolvers for a sub-type of a requested super-type. Sometimes the binding just acts as a factory, other times it maintains scopes for types so that they can be reused. A binding is supposed to be self-containing. That means that a binding should contain no global or static state, but rather rely on hierarchies of bindings that use each other to meet an end goal.

## Goals
An API that...
- Allows static bindings that are verifiable at compile-time.
- Is easy to use and extend with external types
- Is usable through LINQ
- Is modular and non global, no `IKernel` needed.

The biggest goal for the binding API is static typing. That means that
whenever you use a binding, you are required to use it in such a way that the compiler is able to
check for valid usage based on the type system. As a result, we need to find solutions to common problems with resolving values in a static way.

## User API
One of the most important functions is resolving a value. There will always be one specific way to resolve a value, and depending on the specific binding more may be provided.

The primary way to resolve a value from a binding is the `Resolve()` method. It returns a type specified by the binding interface.

```csharp
IBinding<TInterface> binding = Bind<TInterface>().Select(type => new TImplementer());

TInterface resolvedValue = binding.Resolve();
```

An example of a different way to resolve a binding is the `IBindWithCondition<TInterface, TInjectedInto>` interface, which allows specifying how to inject a value into an object of a given type via the `Inject()` method.

### Exceptions
A binding is allowed to throw a couple exceptions when `Resolve()` is called:

- `InvalidOperationException`, Thrown only if the binding is not properly set up.
- `ResolveBindingException`, Thrown only if an error occurred when retrieving a value from a binding. That is, the binding was set up properly, but it was not able to resolve a value for some reason. Acts as a wrapper around whatever other exception was thrown by populating the `InternalException` property.

## Internal APIs

Resolving a binding is entirely up to the implementer of the `IBinding<TInterface>`.
