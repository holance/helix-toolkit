/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Components;
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public class BillboardNode : SceneNode, IBoundable
    {
        /// <summary>
        /// Gets or sets a value indicating whether [fixed size].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed size]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedSize
        {
            set
            {
                (RenderCore as IBillboardRenderParams).FixedSize = value;
            }
            get { return (RenderCore as IBillboardRenderParams).FixedSize; }
        }

        /// <summary>
        /// Gets or sets the sampler description.
        /// </summary>
        /// <value>
        /// The sampler description.
        /// </value>
        public SamplerStateDescription SamplerDescription
        {
            set
            {
                (RenderCore as IBillboardRenderParams).SamplerDescription = value;
            }
            get
            {
                return (RenderCore as IBillboardRenderParams).SamplerDescription;
            }
        }
        public GeometryBoundManager BoundManager { get; }
        public GeometryComponent GeometryComp { get; }
        public IsTransparentComponent IsTransparentComp { get; }
        public RasterStateComponent RasterComp { get; }
        public PostEffectComponent PostEffectComp { get; }

        public sealed override BoundingBox OriginalBounds => BoundManager.OriginalBounds;

        public sealed override BoundingSphere OriginalBoundsSphere => BoundManager.OriginalBoundsSphere;

        public sealed override BoundingBox Bounds => BoundManager.Bounds;

        public sealed override BoundingSphere BoundsSphere => BoundManager.BoundsSphere;

        public sealed override BoundingSphere BoundsSphereWithTransform => BoundManager.BoundsSphereWithTransform;

        public sealed override BoundingBox BoundsWithTransform => BoundManager.BoundsWithTransform;

        public BillboardNode()
        {
            BoundManager = AddComponent(new GeometryBoundManager(this, OnCheckGeometry));
            GeometryComp = AddComponent(new GeometryComponent(this, RenderCore as IGeometryRenderCore, BoundManager, OnCreateBufferModel));
            IsTransparentComp = AddComponent(new IsTransparentComponent(this));
            RasterComp = AddComponent(new RasterStateComponent(RenderCore as IRasterStateParam));
            PostEffectComp = AddComponent(new PostEffectComponent(this));
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new BillboardRenderCore();
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected virtual IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return geometry != null && geometry.IsDynamic ? EffectsManager.GeometryBufferManager.Register<DynamicBillboardBufferModel>(modelGuid, geometry) 
                : EffectsManager.GeometryBufferManager.Register<DefaultBillboardBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BillboardText];
        }

        public override bool TestViewFrustum(ref BoundingFrustum viewFrustum)
        {
            if (!GeometryComp.EnableFrustumCheck)
            {
                return true;
            }
            return BoundingFrustumExtensions.Intersects(ref viewFrustum, ref GeometryComp.BoundManager.BoundsSphereWithTransform);// viewFrustum.Intersects(ref sphere);
        }

        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected virtual bool OnCheckGeometry(Geometry3D geometry)
        {
            return geometry is IBillboardText;
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return (GeometryComp.Geometry as BillboardBase).HitTest(context, totalModelMatrix, ref ray, ref hits, this.WrapperSource, FixedSize);
        }
    }
}
