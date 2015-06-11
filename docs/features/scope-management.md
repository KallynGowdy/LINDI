# Scope Management
This document describes how scopes are defined and managed in bindings.

In LINDI, the `IBinding<TInterface>` is king. Because of its simple nature, we are able to do
a lot of "magic" in a non magical way. For example, managing scopes using bindings is as simple
as wrapping any other binding in a binding that tracks the scope.

The scopes that are provided in LINDI by default are (Checked out ones are implemented):

- [x] Transient, this is the default. (No special wrapper binding is needed)
- [ ] Singleton
- [ ] HTTP Request, A new object is bound per HTTP Request.
- [ ] Thread, A new object is bound per thread. (essentially like using `ThreadLocal`)
- [ ] Named, A new object is created per named scope.

Scopes are usually specified using the `group by` clause in LINQ, however they may also be defined by extra extension methods or by wrapping an expression in the specified scope binding.
Scopes also normally rely on "ambient" context. That is, static information that appears different in different scenarios. Think `HttpContext.Current` or `Thread.CurrentThread`. Those static properties give access to different values in different scenarios. The named thread is a little more difficult, but it too can rely on ambient context through thread safe locks and a stack that tracks each named scope.

Further descriptions are below:

## Transient
This is the default scope because every binding inherits this out of nature. The `BindToConstructor` class just creates new objects out of specified constructors, it has no concept of scopes.

## Singleton
This scope defines that only a single instance of the object should be created in the entire lifetime of the application. These are long lived objects whose purpose in life makes the application run by necessity. You can define a singleton scope by grouping by `true`.

```csharp
IBinding<TInterface> binding = from type in Bind<TInterface>()
                               group by true into scope
                               select new TImplementer();

// or
IBinding<TInterface> binding = from type in Bind<TInterface>()
                               group by Singleton()
                               select new TImplementer();
```

## HTTP Request
This scope defines that a single instance is created per `HttpContext` that the object is requested in. These objects are usually short-lived, only being used during one request and then destroyed.
You can define a HTTP Request scope by grouping by `HttpContext.Current`

```csharp
IBinding<TInterface> binding = from type in Bind<TInterface>()
                               group by HttpContext.Current into scope
                               select new TImplementer();

// or
IBinding<TInterface> binding = from type in Bind<TInterface>()
                               group by HttpRequest()
                               select new TImplementer();
```

## Thread
This scope defines that a single instance is created per thread that the object is requested in. These objects are usually not thread-safe and are difficult to manage across multiple threads. (e.g. highly synchronized) You can define a thread scope by grouping by `Thread.CurrentThread`

```csharp
IBinding<TInterface> binding = from type in Bind<TInterface>()
                               group by Thread.CurrentThread into scope
                               select new TImplementer();

// or
IBinding<TInterface> binding = from type in Bind<TInterface>()
                               group by Thread() into scope
                               select new TImplementer();

```

## Named
Named scopes are an oddity, but I assume that they are useful nonetheless (I've never used them myself, maybe I'm missing out). They are used to provide the same instance to objects that are "grouped" together with the same name, but only to those in the same group. Different named groups get different instances. A named scope can be defined by grouping by a `string`. Named scopes are entered into by using the `Scope()` method on the returned `INamedBinding`. It is important to note

```csharp
INamedBinding<TInterface> binding = from type in Bind<TInterface>()
                               group by Name() into scope
                               select new TImplementer();

// Usage
using(binding.Scope("Name"))
{
  TInterface i = binding.Resolve();
  TInterface i2 = binding.Resolve();

  Assert.Same(i, i2);
}
```
