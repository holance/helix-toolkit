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
    using Model.Scene;
    using Utilities;


    public sealed class BatchedGeometryComponents : EntityComponent, IBoundManager
    {
        #region Properties
        private BatchedMeshGeometryConfig[] geometries;
        public BatchedMeshGeometryConfig[] Geometries
        {
            set
            {
                if (Set(ref geometries, value))
                {
                    BatchedGeometryOctree = null;
                    if (IsAttached)
                    {
                        batchingBuffer.Geometries = value;
                    }
                    UpdateBounds();
                    node.InvalidateRender();
                }
            }
            get
            {
                return geometries;
            }
        }

        private PhongMaterialCore[] materials;
        public PhongMaterialCore[] Materials
        {
            set
            {
                if (Set(ref materials, value) && IsAttached)
                {
                    batchingBuffer.Materials = value;
                    if (value == null && DefaultMaterial is PhongMaterialCore p)
                    {
                        batchingBuffer.Materials = new PhongMaterialCore[] { p };
                    }
                    node.InvalidateRender();
                }
            }
            get { return materials; }
        }

        public PhongMaterialCore DefaultMaterial { set; get; } = new PhongMaterialCore();
        public bool EnableFrustumCheck = true;
        #region Bound
        private static readonly BoundingBox DefaultBound = new BoundingBox();
        private static readonly BoundingSphere DefaultBoundSphere = new BoundingSphere();

        /// <summary>
        /// Gets the original bound from the geometry. Same as <see cref="Geometry3D.Bound"/>
        /// </summary>
        /// <value>
        /// The original bound.
        /// </value>
        public BoundingBox OriginalBounds;

        /// <summary>
        /// Gets the original bound sphere from the geometry. Same as <see cref="Geometry3D.BoundingSphere"/> 
        /// </summary>
        /// <value>
        /// The original bound sphere.
        /// </value>
        public BoundingSphere OriginalBoundsSphere;

        /// <summary>
        /// Gets the bounds. Usually same as <see cref="OriginalBounds"/>. If have instances, the bound will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public BoundingBox Bounds;

        /// <summary>
        /// Gets the bounds with transform. Usually same as <see cref="Bounds"/>. If have transform, the bound is the transformed <see cref="Bounds"/>
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public BoundingBox BoundsWithTransform;

        /// <summary>
        /// Gets the bounds sphere. Usually same as <see cref="OriginalBoundsSphere"/>. If have instances, the bound sphere will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public BoundingSphere BoundsSphere;
        
        /// <summary>
        /// Gets the bounds sphere with transform. If have transform, the bound is the transformed <see cref="BoundsSphere"/>
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public BoundingSphere BoundsSphereWithTransform;

        /// <summary>
        /// Occurs when [on bound changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<BoundingBox>> OnBoundChanged;

        /// <summary>
        /// Occurs when [on transform bound changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<BoundingBox>> OnTransformBoundChanged;

        /// <summary>
        /// Occurs when [on bound sphere changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<BoundingSphere>> OnBoundSphereChanged;

        /// <summary>
        /// Occurs when [on transform bound sphere changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<BoundingSphere>> OnTransformBoundSphereChanged;
        #endregion
        #endregion

        private DefaultStaticMeshBatchingBuffer batchingBuffer;
        private readonly SceneNode node;
        private readonly IGeometryRenderCore core;
        private readonly Func<Guid, Geometry3D, IAttachableBufferModel> createBufferFunc;
        private readonly Func<Geometry3D, bool> checkGeoValidity;
        public StaticBatchedGeometryBoundsOctree BatchedGeometryOctree { private set; get; }

        public BatchedGeometryComponents(SceneNode node, IGeometryRenderCore renderCore)
        {
            this.node = node;
            core = renderCore;
        }

        private void UpdateBounds()
        {
            var oldBound = OriginalBounds;
            var oldBoundSphere = OriginalBoundsSphere;
            if (geometries != null && geometries.Length > 0)
            {
                var b = geometries[0].Geometry.Bound;
                var bs = geometries[0].Geometry.BoundingSphere;
                foreach (var geo in geometries)
                {
                    b = BoundingBox.Merge(b, geo.Geometry.Bound.Transform(geo.ModelTransform));
                    bs = BoundingSphere.Merge(bs, geo.Geometry.BoundingSphere.TransformBoundingSphere(geo.ModelTransform));
                }
                OriginalBounds = b;
                OriginalBoundsSphere = bs;
                BatchedGeometryOctree = new StaticBatchedGeometryBoundsOctree(geometries, new OctreeBuildParameter());
            }
            else
            {
                OriginalBounds = DefaultBound;
                OriginalBoundsSphere = DefaultBoundSphere;
                BatchedGeometryOctree = null;
            }

            RaiseOnBoundChanged(new BoundChangeArgs<BoundingBox>(ref OriginalBounds, ref oldBound));
            RaiseOnBoundSphereChanged(new BoundChangeArgs<BoundingSphere>(ref OriginalBoundsSphere, ref oldBoundSphere));
            UpdateBoundsWithTransform();
        }

        private void UpdateBoundsWithTransform()
        {
            var old = BoundsWithTransform;
            if (OriginalBounds == DefaultBound)
            {
                BoundsWithTransform = DefaultBound;
            }
            else
            {
                BoundsWithTransform = OriginalBounds.Transform(node.TransformComp.TotalModelTransform);
            }
            var oldBS = BoundsSphereWithTransform;
            if (OriginalBoundsSphere == DefaultBoundSphere)
            {
                BoundsSphereWithTransform = DefaultBoundSphere;
            }
            else
            {
                BoundsSphereWithTransform = OriginalBoundsSphere.TransformBoundingSphere(node.TransformComp.TotalModelTransform);
            }
            RaiseOnTransformBoundChanged(new BoundChangeArgs<BoundingBox>(ref BoundsWithTransform, ref old));
            RaiseOnTransformBoundSphereChanged(new BoundChangeArgs<BoundingSphere>(ref BoundsSphereWithTransform, ref oldBS));
        }

        protected override void OnAttach()
        {
            batchingBuffer = Collect(new DefaultStaticMeshBatchingBuffer());
            batchingBuffer.Geometries = Geometries;
            batchingBuffer.Materials = materials;
            core.GeometryBuffer = batchingBuffer;
        }

        protected override void OnDetach()
        {
            core.GeometryBuffer = batchingBuffer = null;
        }

        /// <summary>
        /// Raises the on transform bound changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void RaiseOnTransformBoundChanged(BoundChangeArgs<BoundingBox> args)
        {
            OnTransformBoundChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the on bound changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void RaiseOnBoundChanged(BoundChangeArgs<BoundingBox> args)
        {
            OnBoundChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the on transform bound sphere changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void RaiseOnTransformBoundSphereChanged(BoundChangeArgs<BoundingSphere> args)
        {
            OnTransformBoundSphereChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the on bound sphere changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void RaiseOnBoundSphereChanged(BoundChangeArgs<BoundingSphere> args)
        {
            OnBoundSphereChanged?.Invoke(this, args);
        }

        public void UpdateOctree()
        {
            if(Geometries != null)
            {
                foreach (var geometry in Geometries)
                {
                    geometry.Geometry?.UpdateOctree();
                }
                if (BatchedGeometryOctree != null && !BatchedGeometryOctree.TreeBuilt)
                {
                    BatchedGeometryOctree.BuildTree();
                }
            }
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                OnTransformBoundChanged = OnBoundChanged = null;
                OnBoundSphereChanged = OnTransformBoundSphereChanged = null;
            }
            base.OnDispose(disposeManagedResources);
        }
    }
}
