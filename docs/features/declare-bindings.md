# Declare Bindings
This document represents the description of the feature that allows users of the API to declare bindings between interface types and their respective implementers.

Bindings will normally be declared using a static method that returns a type that can be operated on by the LINQ Extensions.

e.g.:
	
```csharp
public static IBinding<TInterface> Bind<TInterface>()
{
		return null;
}
```


Usage:

```csharp
IBinding<TInterface> b = Bind<TInterface>();
```

An interface type will be bound to a constructor using the `Select()` extension method.
The expressions passed into the select method are used for special "expression-hacks", allowing us to specify that a certain type should be used. In the future, functions may be used and special values that support the operations that we want _might_ be used.

Automatically-chosen constructor:

```csharp
IBinding<TInterface> finalBinding = b.Select(type => type as TImplementor);
IBinding<TInterface> finalBinding = from type in b select type as TImplementor;
```
User-defined constructor:

```csharp
IBinding<TInterface> finalBinding = b.Select(type => new TImplementor());
IBinding<TInterface> finalBinding = from type in b select new TImplementor();
```

Values can be resolved in the constructor using the `default(T)` operator:

```csharp
// Resolve an int in the constructor
IBinding<TInterface> finalBinding = b.Select(type => new TImplementor(default(int)));
IBinding<TInterface> finalBinding = from type in b select new TImplementor(default(int));
```
