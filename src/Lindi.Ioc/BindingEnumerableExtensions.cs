using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lindi.Core;

namespace Lindi.Ioc
{
    public static class BindingEnumerableExtensions
    {
        public static IBindingCollection ToCollection(this IEnumerable<IBinding> bindings)
        {
            return new BindingCollection(bindings);
        }

    }
}
