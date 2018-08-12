/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Components;
    using Core;
    /// <summary>
    /// Static mesh batching. Supports multiple <see cref="Materials"/>. All geometries are merged into single buffer for rendering. Indivisual material color infomations are encoded into vertex buffer.
    /// <para>
    /// <see cref="Material"/> is used if <see cref="Materials"/> = null. And also used for shared material texture binding.
    /// </para>
    /// </summary>
    public class BatchedMeshNode : SceneNode, IHitable, IBoundable
    {
        public BatchedGeometryComponents BatchedGeometryComp { get; }
        public MaterialComponent MaterialComp { get; }
        public RasterStateComponent RasterComp { get; }

        public IsTransparentComponent IsTransparentComp { get; }

        public PostEffectComponent PostEffectComp { get; }

        public ShadowComponent ShadowComp { get; }

        public InvertNormalComponent InvertNormalComp { get; }

        public RenderWireframeComponent WireframeComp { get; }

        public sealed override BoundingBox OriginalBounds => BatchedGeometryComp.OriginalBounds;

        public sealed override BoundingSphere OriginalBoundsSphere => BatchedGeometryComp.OriginalBoundsSphere;

        public sealed override BoundingBox Bounds => BatchedGeometryComp.Bounds;

        public sealed override BoundingSphere BoundsSphere => BatchedGeometryComp.BoundsSphere;

        public sealed override BoundingSphere BoundsSphereWithTransform => BatchedGeometryComp.BoundsSphereWithTransform;

        public sealed override BoundingBox BoundsWithTransform => BatchedGeometryComp.BoundsWithTransform;

        public BatchedMeshNode()
        {
            HasBound = true;
            BatchedGeometryComp = AddComponent(new BatchedGeometryComponents(this, RenderCore as IGeometryRenderCore));
            MaterialComp = AddComponent(new MaterialComponent(this, RenderCore as IMaterialRenderParams, OnCreateMaterial));
            RasterComp = AddComponent(new RasterStateComponent(RenderCore as IRasterStateParam));
            IsTransparentComp = AddComponent(new IsTransparentComponent(this));
            PostEffectComp = AddComponent(new PostEffectComponent(this));
            ShadowComp = AddComponent(new ShadowComponent(RenderCore));
            InvertNormalComp = AddComponent(new InvertNormalComponent(RenderCore as IInvertNormal));
            WireframeComp = AddComponent(new RenderWireframeComponent(RenderCore as IMeshRenderParams));
            MaterialComp.OnMaterialChanged += (s, e) => { BatchedGeometryComp.DefaultMaterial = MaterialComp.Material as PhongMaterialCore; };
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new MeshRenderCore
            {
                Batched = true
            };
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BlinnBatched];
        }

        private MaterialVariable OnCreateMaterial(MaterialCore material)
        {
            return EffectsManager.MaterialVariableManager.Register(material, EffectTechnique);
        }

        protected override OrderKey OnUpdateRenderOrderKey()
        {
            return OrderKey.Create(RenderOrder, MaterialComp.MaterialVariableID);
        }

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context) && BatchedGeometryComp.Geometries != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Views the frustum test.
        /// </summary>
        /// <param name="viewFrustum">The view frustum.</param>
        /// <returns></returns>
        public override bool TestViewFrustum(ref BoundingFrustum viewFrustum)
        {
            if (!BatchedGeometryComp.EnableFrustumCheck)
            {
                return true;
            }
            return BoundingFrustumExtensions.Intersects(ref viewFrustum, ref BatchedGeometryComp.BoundsWithTransform, ref BatchedGeometryComp.BoundsSphereWithTransform);
        }

        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(RenderContext context)
        {
            return base.CanHitTest(context) && BatchedGeometryComp.Geometries != null && BatchedGeometryComp.Geometries.Length > 0 && BatchedGeometryComp.Materials != null && BatchedGeometryComp.Materials.Length > 0;
        }

        /// <summary>
        /// Updates the not render.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void UpdateNotRender(RenderContext context)
        {
            base.UpdateNotRender(context);
            BatchedGeometryComp.UpdateOctree();
        }

        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            if(ray.Intersects(BatchedGeometryComp.BoundsWithTransform) && ray.Intersects(BatchedGeometryComp.BoundsSphereWithTransform))
            {                
                if(BatchedGeometryComp.BatchedGeometryOctree != null && BatchedGeometryComp.BatchedGeometryOctree.TreeBuilt)
                {
                    return BatchedGeometryComp.BatchedGeometryOctree.HitTest(context, WrapperSource, null, totalModelMatrix, ray, ref hits);
                }
                else
                {
                    bool isHit = false;
                    foreach(var geo in BatchedGeometryComp.Geometries)
                    {
                        if(geo.Geometry is MeshGeometry3D mesh)
                        {
                            isHit |= mesh.HitTest(context, geo.ModelTransform * totalModelMatrix, ref ray, ref hits, WrapperSource);
                        }
                    }
                    return isHit;
                }            
            }
            else
            {
                return false;
            }
        }
    }
}
