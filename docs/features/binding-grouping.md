# Grouping Bindings
This document represents the definition of how bindings are grouped together in so called
`"meta-bindings"`.

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

Unfortunately, many existing frameworks and libraries do not request dependencies through a generic type call, but rather via a non-generic call using a `Type` parameter. i.e.

```csharp
Type controllerType = GetControllerTypeForRoute(routeInfo);
Controller controller = (Controller)ResolveController(controllerType);
```

Therefore, we need an API that is able to easily interface with these components while maintaining our ever precious type safety.

## User API

To maintain our type safety, bindings are still defined as normal, but now they can be grouped into a nice, simple collection style object:

```csharp
IBinding<TInterface> firstBinding = from value in Bind<TImplementer>()
                                    select new TImplementer();

IBinding<TOther> secondBinding = from value in Bind<TOther>()
                                 select new TOther();

IBindingCollection bindings = new IBinding[] { firstBinding, secondBinding }.ToCollection();
// or
BindingCollection bindings = new IBinding[] { firstBinding, secondBinding };

// Usage
object obj = bindings.Resolve(typeof(TInterface));
TInterface i = (TInterface)obj;

object[] objs = bindings.ResolveAll(typeof(TInterface));
TInterface[] interfaces = objs.Cast<TInterface>().ToArray();

```

Priority is determined by which binding is defined first in the collection, although that can be changed.


## Internal API
