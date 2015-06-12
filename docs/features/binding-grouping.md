# Grouping Bindings
This document represents the definition of how bindings are grouped together in so called
`"meta-binding"`.

In LINDI, because the `IBinding<TInterface>` only represents a binding for a single type, we need a way to represent multiple bindings as one. The easy way to go about this would be to construct a type that just holds a dictionary of bindings and determines the binding that should be used when called. While simple and potentially quite efficient, it does not match up with the a couple of the stated goals of LINDI. Namely that dependency graphs are verifiable at compile time. So, this document aspires to meet the needs of automatically resolving dependencies while maintaining compile time safety.

As such, a distributed model for bindings is best. The recommended way of resolving dependencies is by using the `let` LINQ clause. It allows you to specify how bindings should be configured and know that your bindings are type-safe. For example, to define that `TInterface` has a dependency on `TDependency` that should be resolved using `DependencyBinding`:

```csharp

IBinding<TDependency> DependencyBinding = from value in Bind<TDependency>()
                                          select new TDependency();

IBinding<TInterface> finalBinding =
  from value in Bind<TInterface>()
  // DependencyUsing() returns the value for the IBinding<T>
  let d = new { dep = DependencyUsing(DependencyBinding) }
  select new TImplementer(d.dep);


// Alternate Syntax
IBinding<TInterface> finalBinding =
  from value in Bind<TInterface>()
  // Named arguments aren't supported in expressions, sadly :(
  select new TImplementer(DependencyUsing(DependencyBinding));


```

Because of limitations in the LINQ expression API, I am not (currently) able to provide support for multiple `let` clauses that directly represent the binding dependency. Therefore, you should just use `anonymous classes` to define your dependencies in one `let` clause. If you prefer, you could just inline the dependency declarations in the constructor.

## User API

Dependencies are defined using the `let` clause from LINQ:

```csharp
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               let d = new { dep = DependencyUsing(bindingImDependentOn) }
                               select new TImplementer(d.dep);
```

Multiple dependencies, multiple properties:

```csharp
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               let d = new
                               {
                                  dep1 = DependencyUsing(bindingImDependentOn),
                                  dep2 = DependencyUsing(bindingImAlsoDependentOn)
                               }
                               select new TImplementer(d.dep1, d.dep2);
```

To allow bindings to have custom parameters:

```csharp
// IBinding<TIn, TOut>
IBinding<DepType, TInterface> binding =
    from value in Bind<TInterface>()
    let d = new
    {
      dep = DependencyUsing(bindingImDependentOn),
      paramDep = Param<DepType>()
    }
    // ...
    select new TImplementer(d.dep, d.paramDep);
```

When being resolved, the given dependencies will be used for those constructor arguments.


## Internal API

To maintain the fastest performance, dependencies are usually built into generated constructor functions. This means that when using the LINQ API for LINDI you are not getting a direct `IBindToConstructor` object back, but rather a lazy object that creates the function and corresponding `IBindToConstructor` object once a value is attempted to be resolved from it.

This is what it looks like:

```csharp
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               let d = new
                               {
                                 dep1 = DependencyUsing(bindingImDependentOn),
                                 dep2 = DependencyUsing(bindingImAlsoDependentOn)
                               }
                               // ...
                               select new TImplementer(d.dep1, d.dep2);

// Equivalent, in reality the anonymous class that is generated is used instead
// of a tuple
IBinding<TInterface> binding =
  new LazyConstructorBinding<TInterface>(
        new IBinding[] { bindingImDependentOn, bindingImAlsoDependentOn },

        // Expression, not func
        v => new TImplementer(v[0].Resolve(...), v[1].Resolve(...))
  );
```
