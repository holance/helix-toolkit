using global::SharpDX.Direct3D11;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Core;

    public sealed class RasterStateComponent : EntityComponent
    {
        #region Rasterizer parameters

        private int depthBias = 0;
        /// <summary>
        /// Gets or sets the depth bias.
        /// </summary>
        /// <value>
        /// The depth bias.
        /// </value>
        public int DepthBias
        {
            set
            {
                if (Set(ref depthBias, value))
                {
                    OnRasterStateChanged();
                }
            }
            get { return depthBias; }
        }

        private float depthBiasClamp;
        /// <summary>
        /// Gets or sets the depth bias clamp.
        /// </summary>
        /// <value>
        /// The depth bias clamp.
        /// </value>
        public float DepthBiasClamp
        {
            set
            {
                if (Set(ref depthBiasClamp, value))
                {
                    OnRasterStateChanged();
                }
            }
            get { return depthBiasClamp; }
        }

        private float slopScaledDepthBias = 0;
        /// <summary>
        /// Gets or sets the slope scaled depth bias.
        /// </summary>
        /// <value>
        /// The slope scaled depth bias.
        /// </value>
        public float SlopeScaledDepthBias
        {
            get { return slopScaledDepthBias; }
            set
            {
                if (Set(ref slopScaledDepthBias, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isMSAAEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether Multisampling Anti-Aliasing enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is msaa enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMSAAEnabled
        {
            get { return isMSAAEnabled = true; }
            set
            {
                if (Set(ref isMSAAEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isScissorEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scissor enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is scissor enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScissorEnabled
        {
            get { return isScissorEnabled; }
            set
            {
                if (Set(ref isScissorEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private FillMode fillMode = FillMode.Solid;
        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        /// <value>
        /// The fill mode.
        /// </value>
        public FillMode FillMode
        {
            get { return fillMode; }
            set
            {
                if (Set(ref fillMode, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isDepthClipEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is depth clip enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is depth clip enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDepthClipEnabled
        {
            get { return isDepthClipEnabled; }
            set
            {
                if (Set(ref isDepthClipEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool frontCCW = true;
        /// <summary>
        /// Gets or sets a value indicating whether [front CCW].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [front CCW]; otherwise, <c>false</c>.
        /// </value>
        public bool FrontCCW
        {
            get { return frontCCW; }
            set
            {
                if (Set(ref frontCCW, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private CullMode cullMode = CullMode.None;
        /// <summary>
        /// Gets or sets the cull mode.
        /// </summary>
        /// <value>
        /// The cull mode.
        /// </value>
        public CullMode CullMode
        {
            get { return cullMode; }
            set
            {
                if (Set(ref cullMode, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        #endregion Rasterizer parameters        

        private readonly IRasterStateParam core;

        public RasterStateComponent(IRasterStateParam renderCore)
        {
            core = renderCore;
            if (core == null)
            {
                throw new ArgumentNullException("Arguments cannot be null.");
            }
        }

        private void OnRasterStateChanged()
        {
            core.RasterDescription = new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode,
                DepthBias = DepthBias,
                DepthBiasClamp = DepthBiasClamp,
                SlopeScaledDepthBias = SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = FrontCCW,
                IsMultisampleEnabled = IsMSAAEnabled,
                IsScissorEnabled = IsScissorEnabled// IsThrowingShadow ? false : IsScissorEnabled,
            };
        }

        protected override void OnAttach()
        {
            OnRasterStateChanged();
        }

        protected override void OnDetach()
        {
        }
    }
}
