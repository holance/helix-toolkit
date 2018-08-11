using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Core;

    public sealed class InvertNormalComponent : EntityComponent
    {
        public bool InvertNormal
        {
            set
            {
                core.InvertNormal = value;
            }
            get
            {
                return core.InvertNormal;
            }
        }

        private readonly IInvertNormal core;

        public InvertNormalComponent(IInvertNormal renderCore)
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
