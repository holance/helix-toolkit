using System;
using global::SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core.Components
#else
namespace HelixToolkit.UWP.Core.Components
#endif
{
    using Model;

    public sealed class MaterialComponent : CoreComponent
    {
        public event EventHandler OnMaterialChanged;
        private MaterialVariable materialVariables = EmptyMaterialVariable.EmptyVariable;
        /// <summary>
        /// Used to wrap all material resources
        /// </summary>
        public MaterialVariable MaterialVariables
        {
            set
            {
                var old = materialVariables;
                if (Set(ref materialVariables, value))
                {
                    if (value == null)
                    {
                        materialVariables = EmptyMaterialVariable.EmptyVariable;
                    }
                }
            }
            get
            {
                return materialVariables;
            }
        }

        protected override void OnAttach(IRenderTechnique technique)
        {

        }

        protected override void OnDetach()
        {

        }
    }
}
