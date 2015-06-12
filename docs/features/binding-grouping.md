# Grouping Bindings
This document represents the definition of how bindings are grouped together in so called
`"meta-binding"`.

In LINDI, because the `IBinding<TInterface>` only represents a binding for a single type, we need a way to represent multiple bindings as one. The easy way to go about this would be to construct a type that just holds a dictionary of bindings and determines the binding that should be used when called. While simple and potentially quite efficient, it does not match up with the a couple of the stated goals of LINDI. Namely that dependency graphs are verifiable at compile time. So, this document aspires to meet the needs of automatically resolving dependencies while maintaining compile time safety.

As such, a distributed model for bindings is best. This means that you define dependencies on specific bindings. It allows you to specify how bindings should be configured and know that your bindings are type-safe. For example, to define that `TInterface` has a dependency on `TDependency` that should be resolved using `DependencyBinding`:

```csharp

IBinding<TDependency> binding = from value in Bind<TDependency>()
                                select new TDependency();

IBinding<TInterface> finalBinding =
              from value in Bind<TInterface>()
              // Named arguments aren't supported in expressions, sadly :(
              select new TImplementer(Dependency(binding));
```

## User API

Dependencies are defined using the `Dependency()` method helper in a constructor:

```csharp
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               select new TImplementer(Dependency(bindingImDependentOn));
```

Multiple dependencies, multiple calls:

```csharp
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               select new TImplementer(
                                  DependencyUsing(bindingImDependentOn),
                                  DependencyUsing(bindingImAlsoDependentOn));
```

When being resolved, the given dependencies will be used for those constructor arguments.


## Internal API

To maintain the fastest performance, dependencies are usually built into generated constructor functions. This means that when using the LINQ API for LINDI you are not getting a direct `IBindToConstructor` object back, but rather a lazy object that creates the function and corresponding `IBindToConstructor` object once a value is attempted to be resolved from it.

This is what it looks like:

```csharp
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               // ...
                               select new TImplementer(
                                  DependencyUsing(bindingImDependentOn),
                                  DependencyUsing(bindingImAlsoDependentOn));

// Equivalent, in reality the anonymous class that is generated is used instead
// of a tuple
IBinding<TInterface> binding =
  new LazyConstructorBinding<TInterface>(
        new IBinding[] { bindingImDependentOn, bindingImAlsoDependentOn },

        // Expression, not func
        v => new TImplementer(v[0].Resolve(...), v[1].Resolve(...))
  );
```
