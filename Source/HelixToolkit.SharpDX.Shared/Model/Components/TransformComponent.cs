
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
        public event EventHandler OnTransformChanged;

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

        private TransformComponent parent;
        public TransformComponent Parent
        {
            set
            {
                if(Set(ref parent, value))
                {
                    NeedsRecompute = true;
                }
            }
            get { return parent; }
        }

        public List<TransformComponent> Children = new List<TransformComponent>();

        public bool NeedsRecompute = true;

        private Matrix totalModelTransform = Matrix.Identity;
        public Matrix TotalModelTransform
        {
            set
            {
                if(Set(ref totalModelTransform, value))
                {
                    for(int i=0; i < Children.Count; ++i)
                    {
                        Children[i].NeedsRecompute = true;
                    }
                    NeedsRecompute = false;
                    OnTransformChanged.Invoke(this, EventArgs.Empty);
                }
            }
            get { return totalModelTransform; }
        }

        public void AddChild(TransformComponent component)
        {
            if(component.parent != null && component.parent != this)
            {
                throw new ArgumentException("TransformComponent.Parent is attached to different TransformComponent.");
            }
            component.Parent = this;
            Children.Add(component);
        }

        public void RemoveChild(TransformComponent component)
        {
            Children.Remove(component);
            component.Parent = null;
        }

        public void Compute()
        {
            TotalModelTransform = modelMatrix * parent.totalModelTransform;
        }

        protected override void OnAttach()
        {
            NeedsRecompute = true;
        }

        protected override void OnDetach()
        {
        }
    }
}
