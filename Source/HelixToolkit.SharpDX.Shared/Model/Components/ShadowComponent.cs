
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Core;

    public sealed class ShadowComponent : EntityComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is throwing shadow.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is throwing shadow; otherwise, <c>false</c>.
        /// </value>
        public bool IsThrowingShadow
        {
            set
            {
                core.IsThrowingShadow = value;
            }
            get
            {
                return core.IsThrowingShadow;
            }
        }

        private readonly RenderCore core;

        public ShadowComponent(RenderCore core)
        {
            this.core = core;
            if (core == null)
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
