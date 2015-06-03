# Conditional Bindings
This document represents the description of the feature that allows users of the API
to declare conditional bindings. That is, a binding that only applies when a certian condition is true.

## User API
Conditional bindings are defined using the `where` clause in LINQ. They operate on
`IBinding<TInterface>` objects, and are defined by expressions which are able to resolve
special behavior depending on the usage.

For example, to define a binding that only applies when being injected into a certain type:

```csharp
// InjectedInto() is a special method that acts as an identifier
// for the expression resolution.
IBinding<TInterface> binding = Bind<TInterface>()
                               .Where(type => InjectedInto(type) as TInjectedInto)
                               .Select(value => value.Property = new TImplementer());

// The `as` keyword is used to allow the compiler to retrieve the return type
// from the `where` clause
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               where InjectedInto(value) as TInjectedInto
                               select value.Property = new TImplementer();

// Other Equivalent Syntax
IBinding<TInterface> binding = from value in Bind<TInterface>()
                               where IsInjectedInto<TInjectedInto>()
                               select value.Property = new TImplementer();


TInjectedInto iNeedDependency = ...;
binding.Inject(iNeedDependency);
```

The advanced features will use expressions to determine how the bindings will function, though it
should be possible to take advantage of all of the features through normal delegates too.

There will be several conditional helpers, including:

- [ ] `InjectedInto(value)`
- More to Come :)

**&lt;proposed-feature&gt;**

You can group multiple conditional bindings together using the `Concat()` extension method:

```csharp
IBinding<TInterface> firstBinding = from value in Bind<TInterface>()
                                    where InjectedInto(value) as TInjectedInto
                                    select value.Property = new TImplementer();

IBinding<TInterface> secondBinding = from value in Bind<TInterface>()
                                     where InjectedInto(value) as TOtherInjectedInto
                                     select value.OtherProperty = new TOtherImplementer();


// The first binding will be checked for a match, followed by the second.
IBinding<TInterface> finalBinding = firstBinding.Concat(secondBinding);
```
**&lt;/proposed-feature&gt;**


## Internal API

The internal API will follow the same LINQ-style by passing previous values to an object that represents the conditional binding. There will be a `IBindWithCondition<TInterface>` type that allows the specification of the conditions. In particular, it will provide a `Inject()` function that allows the injection to be statically typed.

```csharp
IBinding<TInterface> finalBinding = Bind<TInterface>()
                                    .Where(type => InjectedInto(type) as TInjectedInto)
                                    .Select(value => value.Property = new TImplementer());

// Equivalent
IBinding<TInterface> finalBinding =
  new BindToInjection<TInterface, TInjectedInto, TImplementer>(
    new BindWithCondition<TInterface, TInjectedInto>(Bind<TInterface>()),
    (Func<TInjectedInto, TImplementer>) value => value.Property = new TImplementer()
  );
```
