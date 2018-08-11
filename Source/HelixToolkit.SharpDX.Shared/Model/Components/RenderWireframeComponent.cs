using System;

using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Core;

    public sealed class RenderWireframeComponent : EntityComponent
    {
        public bool RenderWireframe
        {
            set => core.RenderWireframe = value;
            get => core.RenderWireframe;
        }

        public Color4 WireframeColor
        {
            set => core.WireframeColor = value;
            get => core.WireframeColor;
        }

        private readonly IMeshRenderParams core;
        public RenderWireframeComponent(IMeshRenderParams renderCore)
        {
            core = renderCore;
            if(core == null)
            {
                throw new ArgumentNullException();
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
