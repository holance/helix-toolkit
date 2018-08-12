using System;
using global::SharpDX;
using System.Collections.Generic;
using System.Runtime.Serialization;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Core;   
    using Scene;  

    public sealed class GeometryChangedArgs : EventArgs
    {
        public readonly Geometry3D NewGeometry;
        public readonly Geometry3D OldGeometry;
        public GeometryChangedArgs(Geometry3D newGeo, Geometry3D oldGeo)
        {
            NewGeometry = newGeo;
            OldGeometry = oldGeo;
        }
    }

    [DataContract]
    public sealed class GeometryComponent : EntityComponent
    {
        private Geometry3D geometry;
        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        [DataMember]
        public Geometry3D Geometry
        {
            set
            {
                var old = geometry;
                if (Set(ref geometry, value))
                {
                    BoundManager.Geometry = value;
                    if (node.IsAttached)
                    {
                        CreateGeometryBuffer();
                    }
                    OnGeometryChanged?.Invoke(this, new GeometryChangedArgs(value, old));
                    node.InvalidateRender();
                }
            }
            get
            {
                return geometry;
            }
        }

        private IList<Matrix> instances;
        /// <summary>
        /// Gets or sets the instances.
        /// </summary>
        /// <value>
        /// The instances.
        /// </value>
        [DataMember]
        public IList<Matrix> Instances
        {
            set
            {
                if (Set(ref instances, value))
                {
                    BoundManager.Instances = value;
                    InstanceBuffer.Elements = value;
                    OnInstanceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get { return instances; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable frustum check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable frustum check]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool EnableFrustumCheck = true;
        /// <summary>
        /// Gets a value indicating whether this instance has instances.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has instances; otherwise, <c>false</c>.
        /// </value>
        [IgnoreDataMember]
        public bool HasInstances { get { return InstanceBuffer.HasElements; } }

        /// <summary>
        /// Gets the instance buffer.
        /// </summary>
        /// <value>
        /// The instance buffer.
        /// </value>
        private readonly MatrixInstanceBufferModel InstanceBuffer = new MatrixInstanceBufferModel();

        #region Events
        public event EventHandler OnInstanceChanged;
        public event EventHandler<GeometryChangedArgs> OnGeometryChanged;
        #endregion Events
        /// <summary>
        /// Gets or sets the bound manager.
        /// </summary>
        /// <value>
        /// The bound manager.
        /// </value>
        public GeometryBoundManager BoundManager { get; }
        private readonly SceneNode node;
        private readonly IGeometryRenderCore core;
        private readonly Func<Guid, Geometry3D, IAttachableBufferModel> createBufferFunc;
        private IAttachableBufferModel bufferModelInternal;
        /// <summary>
        /// Gets a value indicating whether [geometry valid].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [geometry valid]; otherwise, <c>false</c>.
        /// </value>
        public bool GeometryValid { get { return BoundManager.GeometryValid; } }       

        public GeometryComponent(SceneNode node, IGeometryRenderCore renderCore, GeometryBoundManager boundManager,
            Func<Guid, Geometry3D, IAttachableBufferModel> createBufferFunction)
        {
            this.node = node;
            core = renderCore;
            BoundManager = boundManager;
            BoundManager.Geometry = geometry;
            this.createBufferFunc = createBufferFunction;
            if (node == null || core == null || createBufferFunc == null)
            {
                throw new ArgumentNullException("Arguments cannot be null.");
            }
        }

        private void CreateGeometryBuffer()
        {
            RemoveAndDispose(ref bufferModelInternal);
            bufferModelInternal = Collect(createBufferFunc(this.GUID, geometry));
            core.GeometryBuffer = bufferModelInternal;
        }

        protected override void OnAttach()
        {
            CreateGeometryBuffer();
            BoundManager.Geometry = geometry;
            InstanceBuffer.Initialize();
            InstanceBuffer.Elements = instances;
            core.InstanceBuffer = InstanceBuffer;
        }

        protected override void OnDetach()
        {
            bufferModelInternal = null;
            InstanceBuffer.DisposeAndClear();
            BoundManager.DisposeAndClear();
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                BoundManager.Dispose();
                InstanceBuffer.Dispose();
            }
            base.OnDispose(disposeManagedResources);
        }
    }
}
