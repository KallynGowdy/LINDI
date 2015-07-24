# LINDI
Language Integrated Dependency Injection ([LINQified][linq] [DI][di])

[![Build status](https://ci.appveyor.com/api/projects/status/hkrqe6rakd7kuqbp/branch/master?svg=true&passingText=master%20|%20Passing&pendingText=master%20|%20Pending&failingText=master%20|%20Failing)](https://ci.appveyor.com/project/KallynGowdy/lindi/branch/master) [![Coverage Status](https://coveralls.io/repos/KallynGowdy/LINDI/badge.svg?branch=master)](https://coveralls.io/r/KallynGowdy/LINDI?branch=master)

Better and more natural way of defining dependencies in .Net, LINDI is a language extension that takes advantage of [LINQ][linq] to specify Dependency Resolution.

#Example

```csharp
using Lindi.Core;
using Lindi.Core.Bindings;
using Lindi.Core.Linq;
using static Lindi.Core.LindiMethods; // C# 6
// ...
// Basic Binding (Bind IFoo to Foo with "name")
var fooBinding = from foo in Bind<IFoo>()
                 select new Foo("name");

IFoo foo = fooBinding.Resolve();

Assert.NotNull(foo);
Assert.Equal("name", foo.Name);

// Dependency Resolution (Bind IBar to Bar using fooBinding)
var barBinding = from bar in Bind<IBar>()
                 select new Bar(Dependency(fooBinding));

IBar bar = barBinding.Resolve();

Assert.NotNull(bar);
Assert.NotNull(bar.Foo);
Assert.Equal("name", bar.Foo);

// Scopes (Bind IFoo to Foo using and reusing foo)
var otherFooBinding = from foo in Bind<IFoo>()
                      group by Singleton() into scope
                      select foo;

var otherBarBinding = from bar in Bind<IBar>()
                      select new Bar(Dependency(otherFooBinding));

IBar otherBar1 = otherBarBinding.Resolve();
IBar otherBar2 = otherBarBinding.Resolve();

Assert.Same(foo, otherBar1.Foo);
Assert.Same(foo, otherBar2.Foo);

// LINQ method syntax works too!
var anotherFooBinding = Bind<IFoo>().Select(foo => new Foo("I'm a Foo!"));
anotherFooBinding = from foo in Bind<IFoo>() select new Foo("I'm a Foo!");

// Interfaces and classes (POCOs)...
public interface IFoo
{
    string Name { get; }
}

public class Foo : IFoo
{
    public Foo(string name)
    {
      Name = name;
    }

    public string Name { get; private set; }
}

public interface IBar
{
    IFoo Foo { get; }
}

public class Bar : IBar
{
    public Bar(IFoo foo)
    {
      Foo = foo;
    }

    public IFoo Foo { get; private set; }
}
```

#Features

- [x] Create bindings between super types and their respective derived types
- [x] Bindings created using LINQ expressions are inlined for performance benefits
- [ ] Conditional Bindings
- [x] Binding Scopes
- [ ] Automatic Factory Generation
- [x] `await` keyword for resolving dependencies

## Issues
Submit new issues as needed, reuse non-closed issues if possible.

# LICENSE
See [LICENSE][license].

    Copyright 2015 Kallyn Gowdy
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.


[di]: http://en.wikipedia.org/wiki/Dependency_injection
[linq]: https://msdn.microsoft.com/en-us/library/bb397926.aspx
[ioc]: http://en.wikipedia.org/wiki/Inversion_of_control
[pull-request]: https://help.github.com/articles/using-pull-requests/
[master-branch]: https://github.com/KallynGowdy/LINDI/tree/master
[license]: https://raw.githubusercontent.com/KallynGowdy/LINDI/master/LICENSE
