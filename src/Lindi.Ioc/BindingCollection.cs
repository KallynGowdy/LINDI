using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lindi.Core;
using Lindi.Core.Bindings;

namespace Lindi.Ioc
{
    /// <summary>
    /// Defines a class that represents objects that contain <see cref="IBinding"/> objects.
    /// </summary>
    public class BindingCollection : IBindingCollection
    {
        private readonly MultiValueDictionary<Type, IBinding> dictionary;

        public BindingCollection()
        {
            dictionary = new MultiValueDictionary<Type, IBinding>();
        }

        public BindingCollection([CanBeNull] IEnumerable<IBinding> bindings)
        {
            dictionary = bindings != null ? new MultiValueDictionary<Type, IBinding>(bindings.GroupBy(b => b.BindingType)) : new MultiValueDictionary<Type, IBinding>();
        }

        public IEnumerator<IBinding> GetEnumerator()
        {
            return dictionary.SelectMany(kv => kv.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int TypeCount => dictionary.Count;
        public int BindingCount => dictionary.Sum(v => v.Value.Count);

        public void Add(IBinding binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));
            dictionary.Add(binding.BindingType, binding);
        }

        public void AddRange(IEnumerable<IBinding> bindings)
        {
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));
            foreach (IBinding binding in bindings)
            {
                Add(binding);
            }
        }

        public bool Remove(IBinding binding)
        {
            return binding != null && dictionary.Remove(binding.BindingType, binding);
        }

        public object Resolve(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            try
            {
                return dictionary[type].First().Resolve();
            }
            catch (KeyNotFoundException ex)
            {
                throw new BindingResolutionException(type, ex);
            }
        }

        public T Resolve<T>()
        {
            Type type = typeof(T);
            try
            {
                IBinding<T> binding = (IBinding<T>)dictionary[type].First();
                return binding.Resolve();
            }
            catch (KeyNotFoundException ex)
            {
                throw new BindingResolutionException(type, ex);
            }
        }

        public object[] ResolveAll(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            try
            {
                return dictionary[type].Select(b => b.Resolve()).ToArray();
            }
            catch (KeyNotFoundException ex)
            {
                throw new BindingResolutionException(type, ex);
            }
        }

        public T[] ResolveAll<T>()
        {
            Type type = typeof(T);
            try
            {
                return dictionary[type].Cast<IBinding<T>>().Select(b => b.Resolve()).ToArray();
            }
            catch (KeyNotFoundException ex)
            {
                throw new BindingResolutionException(type, ex);
            }
        }

        public static implicit operator BindingCollection(IBinding[] bindings)
        {
            return new BindingCollection(bindings);
        }

        public static implicit operator BindingCollection(List<IBinding> bindings)
        {
            return new BindingCollection(bindings);
        }
        
    }
}
