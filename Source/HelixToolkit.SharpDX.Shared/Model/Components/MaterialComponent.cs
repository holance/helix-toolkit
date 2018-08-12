
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Core;
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    public sealed class MaterialComponent : EntityComponent
    {
        public event EventHandler OnMaterialChanged;

        private MaterialVariable materialVariable;
        private MaterialCore material;
        /// <summary>
        ///
        /// </summary>
        public MaterialCore Material
        {
            get { return material; }
            set
            {
                if (Set(ref material, value) && node.IsAttached)
                {
                    AttachMaterial();                    
                    node.InvalidateRender();
                }
            }
        }

        public ushort MaterialVariableID => materialVariable == null ? (ushort)0 : materialVariable.ID;

        private readonly IMaterialRenderParams core;
        private readonly Func<MaterialCore, MaterialVariable> createMaterialVarFunc;
        private readonly SceneNode node;
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialComponent"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="renderCore">The render core.</param>
        /// <param name="createMaterialVarFunction">The create material variable function.</param>
        public MaterialComponent(SceneNode node, IMaterialRenderParams renderCore, Func<MaterialCore, MaterialVariable> createMaterialVarFunction)
        {
            this.node = node;
            core = renderCore;
            createMaterialVarFunc = createMaterialVarFunction;
            if (node == null || core == null || createMaterialVarFunc == null)
            {
                throw new ArgumentNullException("Arguments cannot be null.");
            }
        }
        /// <summary>
        /// Attaches the material.
        /// </summary>
        private void AttachMaterial()
        {
            RemoveAndDispose(ref materialVariable);
            core.MaterialVariables = null;
            if (material != null)
            {
                materialVariable = core.MaterialVariables = Collect(createMaterialVarFunc(material));
            }
            OnMaterialChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Attaches this instance.
        /// </summary>
        protected override void OnAttach()
        {
            AttachMaterial();
        }
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        protected override void OnDetach()
        {
            core.MaterialVariables = materialVariable = null;
        }

        public static implicit operator MaterialCore(MaterialComponent comp)
        {
            return comp.material;
        }
    }
}
