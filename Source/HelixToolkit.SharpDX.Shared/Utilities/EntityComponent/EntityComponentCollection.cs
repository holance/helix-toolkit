using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public sealed class EntityComponentCollection : DisposeObject, IList<EntityComponent>
    {
        private readonly List<EntityComponent> components = new List<EntityComponent>();

        public EntityComponent this[int index] { get => components[index]; set => SetComponent(index, value); }

        public bool IsReadOnly => false;

        public void Add(EntityComponent item)
        {
            components.Add(Collect(item));
        }

        public void Clear()
        {
            components.Clear();
            DisposeAndClear();
        }

        public bool Contains(EntityComponent item)
        {
            return ContainsDisposible(item);
        }

        public void CopyTo(EntityComponent[] array, int arrayIndex)
        {
            for(int i=0; i < components.Count; ++i)
            {
                array[arrayIndex++] = components[i];
            }
        }

        public IEnumerator<EntityComponent> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        public int IndexOf(EntityComponent item)
        {
            return components.IndexOf(item);
        }

        public void Insert(int index, EntityComponent item)
        {
            components.Insert(index, Collect(item));
        }

        public bool Remove(EntityComponent item)
        {
            RemoveAndDispose(ref item);
            return components.Remove(item);
        }

        public void RemoveAt(int index)
        {
            var item = components[index];
            RemoveAndDispose(ref item);
            components.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return components.GetEnumerator();
        }

        public void SetComponent(int index, EntityComponent component)
        {
            var old = components[index];
            if (old != component)
            {
                RemoveAndDispose(ref old);
                components[index] = component;
            }
        }

        /// <summary>
        /// Gets the first component of the specified type or derived type.
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <returns>The first component or null if it was not found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>() where T : EntityComponent
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is T item)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
