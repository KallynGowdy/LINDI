# Declare Bindings
This document represents the description of the feature that allows users of the API to declare bindings between interface types and their respective implementers.

## User API

Bindings will normally be declared using a static method that returns a type that can be operated on by the LINQ Extensions.


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

## Examples

<strike>
Automatically-chosen constructor:

```csharp
IBinding<TInterface> finalBinding = b.Select(type => type as TImplementer);
IBinding<TInterface> finalBinding = from type in b select type as TImplementer;
```
</strike>

User-defined constructor:

```csharp
IBinding<TInterface> finalBinding = b.Select(type => new TImplementer());
IBinding<TInterface> finalBinding = from type in b select new TImplementer();
```

Values can be resolved in the constructor using the `default(T)` operator:

```csharp
// Resolve an int in the constructor
IBinding<TInterface> finalBinding = b.Select(type => new TImplementer(default(int)));
IBinding<TInterface> finalBinding = from type in b select new TImplementer(default(int));
```


## Internal API


~~Automatic bindings will be declared using an implementation of the `IBindToType<TInterface, TImplementer>` interface, which will normally be implemented by `BindToType<TInterface, TImplementer>` class.~~

~~Usage:~~
<strike>
```csharp
IBinding<TInterface> b = Bind<TInterface>().Select(type => type as TImplementer);

// Equivalent
IBinding<TInterface> b = new BindToType<TInterface, TImplementer>(Bind<TInterface>());
```
</strike>

Constructor bindings will be declared using an implementation of the `IBindToConstructor<TInterface, TImplementer>` interface, which will normally be implemented by `BindToConstructor<TInterface, TImplementer>` class.
In order to resolve dependencies effeciently, no `IKernel` or `IResolver` or `ILocator` interface will be provided to the constructor function, thereby reducing the need for dictionary lookups and reflection calls. Rather, when an object needs to be resolved a function is generated that contains all of the direct calls to the required constructors.

```csharp
IBinding<TInterface> finalBinding = b.Select(type => new TImplementer());

// Equivalent
IBinding<TInterface> finalBinding = new BindToConstructor<TInterface, TImplementer>(
	b,
	(Func<TImplementer>) () => new TImplementer()
);
```
