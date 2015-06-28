# LINDI
Language Integrated Dependency Injection ([LINQified][linq] [DI][di])

[![Build status](https://ci.appveyor.com/api/projects/status/hkrqe6rakd7kuqbp/branch/master?svg=true&passingText=master%20|%20Passing&pendingText=master%20|%20Pending&failingText=master%20|%20Failing)](https://ci.appveyor.com/project/KallynGowdy/lindi/branch/master) [![Coverage Status](https://coveralls.io/repos/KallynGowdy/LINDI/badge.svg?branch=master)](https://coveralls.io/r/KallynGowdy/LINDI?branch=master)

Better and more natural way of defining dependencies in .Net, LINDI is a language extension that takes advantage of [LINQ][linq] to specify Dependency Resolution.

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
