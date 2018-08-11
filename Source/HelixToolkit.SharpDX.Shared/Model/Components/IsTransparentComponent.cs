using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Model.Scene;
    

    public sealed class IsTransparentComponent : EntityComponent
    {
        private bool isTransparent = false;
        /// <summary>
        /// Specifiy if model material is transparent.
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get { return isTransparent; }
            set
            {
                if (Set(ref isTransparent, value))
                {
                    if (node.RenderType == RenderType.Opaque || node.RenderType == RenderType.Transparent)
                    {
                        node.RenderType = value ? RenderType.Transparent : RenderType.Opaque;
                    }
                }
            }
        }

        private readonly SceneNode node;

        public IsTransparentComponent(SceneNode node)
        {
            this.node = node;
            if (node == null)
            {
                throw new ArgumentNullException("Arguments cannot be null.");
            }
        }

        protected override void OnAttach()
        {
        }

        protected override void OnDetach()
        {
        }
    }
}
