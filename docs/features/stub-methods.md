# Stub Methods
This document describes the different expression `'stub'` methods and how they are used.

In LINDI, expressions are used extensively to manipulate bindings. These expressions are augmented with `'stub'` methods. These stub methods provide extra context to the builder APIs that convert your expressions into full bindings. There is nothing special about a stub method. In fact, these methods don't contain _any_ code at all. They are identified by the runtime by the attributes that are applied to them. For example, the `Bind<TInterface>` method has the `BindMethodAttribute` applied to it. If you want to create your own stub methods, follow the same lead.
