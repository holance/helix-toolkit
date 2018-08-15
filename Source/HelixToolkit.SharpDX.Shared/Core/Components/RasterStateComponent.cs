using System;
using global::SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core.Components
#else
namespace HelixToolkit.UWP.Core.Components
#endif
{   
    using Utilities;
    public sealed class RasterStateComponent : CoreComponent
    {
        public event EventHandler OnRasterStateChanged;

        private RasterizerStateProxy rasterState;
        public RasterizerStateProxy RasterState { get { return rasterState; } }


        private RasterizerStateDescription rasterDescription = new RasterizerStateDescription()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.None,
        };
        /// <summary>
        /// 
        /// </summary>
        public RasterizerStateDescription RasterDescription
        {
            set
            {
                if (SetAffectsRender(ref rasterDescription, value) && IsAttached)
                {
                    CreateRasterState(value);
                }
            }
            get
            {
                return rasterDescription;
            }
        }

        protected override void OnAttach(IRenderTechnique technique)
        {
            CreateRasterState(rasterDescription);
        }

        protected override void OnDetach()
        {
            rasterState = null;
        }

        private void CreateRasterState(RasterizerStateDescription description)
        {
            RemoveAndDispose(ref rasterState);
            rasterState = Collect(Technique.EffectsManager.StateManager.Register(description));
            OnRasterStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
