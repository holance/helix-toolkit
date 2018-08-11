using System;
using System.Collections;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public abstract class Entity : DisposeObject, IEnumerable<EntityComponent>, IGUID
    {
        public abstract Guid GUID { get; }

        public EntityComponentCollection Components { get; }


        public Entity()
        {
            Components = Collect(new EntityComponentCollection());
        }

        /// <summary>
        /// Gets or create a component with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the entity component</typeparam>
        /// <returns>A new or existing instance of {T}</returns>
        public T GetOrCreate<T>() where T : EntityComponent, new()
        {
            var component = Components.Get<T>();
            if (component == null)
            {
                component = new T();
                Components.Add(component);
            }
            component.Entity = this;
            return component;
        }

        /// <summary>
        /// Adds the specified component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The component.</param>
        /// <returns></returns>
        public T AddComponent<T>(T component) where T : EntityComponent
        {
            Components.Add(component);
            return Collect(component);
        }

        /// <summary>
        /// Gets first component by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Get<T>() where T : EntityComponent
        {
            return Components.Get<T>();
        }

        public IEnumerator<EntityComponent> GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Components.GetEnumerator();
        }
    }
}
