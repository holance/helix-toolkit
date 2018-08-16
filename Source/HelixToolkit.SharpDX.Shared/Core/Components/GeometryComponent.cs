using System;
using global::SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core.Components
#else
namespace HelixToolkit.UWP.Core.Components
#endif
{
    public sealed class GeometryComponent : CoreComponent
    {
        public event EventHandler OnGeometryBufferChanged;
        public event EventHandler OnInstanceBufferChanged;
        public event EventHandler OnInstanceElementChanged;

        private IElementsBufferModel instanceBuffer;
        /// <summary>
        /// 
        /// </summary>
        public IElementsBufferModel InstanceBuffer
        {
            set
            {
                var old = instanceBuffer;
                if (Set(ref instanceBuffer, value))
                {
                    if (old != null)
                    {
                        old.OnElementChanged -= Instance_OnElementChanged;
                    }
                    if (value != null)
                    {
                        value.OnElementChanged += Instance_OnElementChanged;
                    }
                    OnInstanceBufferChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get
            {
                return instanceBuffer;
            }
        }

        private void Instance_OnElementChanged(object sender, EventArgs e)
        {
            OnInstanceElementChanged?.Invoke(this, EventArgs.Empty);
        }

        private IAttachableBufferModel geometryBuffer;
        /// <summary>
        /// 
        /// </summary>
        public IAttachableBufferModel GeometryBuffer
        {
            set
            {
                if (Set(ref geometryBuffer, value))
                {
                    OnGeometryBufferChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get { return geometryBuffer; }
        }

        protected override void OnAttach(IRenderTechnique technique)
        {
        }

        protected override void OnDetach()
        {
        }
    }
}
