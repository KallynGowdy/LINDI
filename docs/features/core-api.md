# Core API
This document represents the definition of the core features and goals of LINDI.

## Goals
- A well-performing and reliable [Inversion of Control][ioc] library.
- A easy-to-use and easy-to-understand [API][api] that leverages the fluent syntax of [LINQ][linq]
- A simplified API that realizes the difficulties of building a usable, production ready library and that compensates by cutting extensive and complicated features. 

# Mandatory Features
- The ability to declare bindings between a base type and an implementer.
- The ability to declare conditional bindings between a base type and an implementer.
- The ability to inject values into a constructor of a requested type.
- The ability to inject values into a property of a requested type.
- The ability to create default factories for resolving instances of types as per the rules defined in the binding.
- The ability to verify bindings immediately at runtime and provide sensible errors when a binding is not provided.
- The ability to generate code (either at runtime or compile time) for factories and injectors so that reflection is not constantly used and that environments that do not support reflection can be supported.

Each of the features described above will be described in-depth in their respective feature files.

[ioc]: http://en.wikipedia.org/wiki/Inversion_of_control
[linq]: https://msdn.microsoft.com/en-us/library/bb397926.aspx
[api]: http://en.wikipedia.org/wiki/Application_programming_interface