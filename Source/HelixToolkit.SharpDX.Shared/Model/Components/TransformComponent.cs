
using System;
using System.Collections.Generic;
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    public sealed class TransformComponent : EntityComponent
    {
        public event EventHandler<TransformArgs> OnTransformChanged;

        private Matrix modelMatrix = Matrix.Identity;
        public Matrix ModelMatrix
        {
            set
            {
                if(Set(ref modelMatrix, value))
                {
                    NeedsRecompute = true;
                }
            }
            get
            {
                return modelMatrix;
            }
        }
        private static readonly TransformComponent DummyParent = new TransformComponent();
        private TransformComponent parent = DummyParent;
        public TransformComponent Parent
        {
            set
            {
                if(Set(ref parent, value))
                {
                    NeedsRecompute = true;
                    if(value == null)
                    {
                        parent = DummyParent;
                    }
                }
            }
            get { return parent; }
        }

        public readonly List<TransformComponent> Children = new List<TransformComponent>();

        public bool NeedsRecompute = true;
        public bool TotalModelTransformChanged = false;
        /// <summary>
        /// The total model transform, updates by <see cref="Compute"/>
        /// </summary>
        public Matrix TotalModelTransform = Matrix.Identity;

        public void AddChild(TransformComponent component)
        {
            if(component.parent != this && component.parent != DummyParent)
            {
                throw new ArgumentException("TransformComponent.Parent is attached to different TransformComponent.");
            }
            component.Parent = this;
            component.NeedsRecompute = true;
            Children.Add(component);
        }

        public void RemoveChild(TransformComponent component)
        {
            Children.Remove(component);
            component.Parent = null;
        }

        public void Compute(bool force = false)
        {
            var total = modelMatrix * parent.TotalModelTransform;
            if(total != TotalModelTransform || force)
            {
                TotalModelTransform = total;
                for (int i = 0; i < Children.Count; ++i)
                {
                    Children[i].NeedsRecompute = true;
                }
                NeedsRecompute = false;
                TotalModelTransformChanged = true;
            }
        }

        public void RaiseTransformChanged()
        {
            OnTransformChanged?.Invoke(this, new TransformArgs(TotalModelTransform));
            TotalModelTransformChanged = false;
        }

        protected override void OnAttach()
        {
            NeedsRecompute = true;
            TotalModelTransform = Matrix.Identity;
        }

        protected override void OnDetach()
        {
        }
    }
}
