using System;
using System.Runtime.Serialization;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Core;   
    public sealed class DynamicReflectorComponent : EntityComponent
    {        
        public IDynamicReflector DynamicReflector
        {
            set
            {
                core.DynamicReflector = value;
            }
            get
            {
                return core.DynamicReflector;
            }
        }

        private readonly IDynamicReflectable core;
        public DynamicReflectorComponent(IDynamicReflectable renderCore)
        {
            core = renderCore;
            if (core == null)
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
