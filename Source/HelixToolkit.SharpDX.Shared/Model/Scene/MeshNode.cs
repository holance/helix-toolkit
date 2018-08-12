/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
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
    ///
    /// </summary>
    public class MeshNode : SceneNode, IBoundable
    {
        public GeometryBoundManager BoundManager { get; }
        public GeometryComponent GeometryComp { get; }
        public MaterialComponent MaterialComp { get; }
        public RasterStateComponent RasterComp { get; }

        public IsTransparentComponent IsTransparentComp { get; }

        public PostEffectComponent PostEffectComp { get; }

        public ShadowComponent ShadowComp { get; }

        public InvertNormalComponent InvertNormalComp { get; }

        public RenderWireframeComponent WireframeComp { get; }

        public sealed override BoundingBox OriginalBounds => BoundManager.OriginalBounds;

        public sealed override BoundingSphere OriginalBoundsSphere => BoundManager.OriginalBoundsSphere;

        public sealed override BoundingBox Bounds => BoundManager.Bounds;

        public sealed override BoundingSphere BoundsSphere => BoundManager.BoundsSphere;

        public sealed override BoundingSphere BoundsSphereWithTransform => BoundManager.BoundsSphereWithTransform;

        public sealed override BoundingBox BoundsWithTransform => BoundManager.BoundsWithTransform;

        public MeshNode()
        {
            BoundManager = AddComponent(new GeometryBoundManager(this, OnCheckGeometry));
            GeometryComp = AddComponent(new GeometryComponent(this, RenderCore as IGeometryRenderCore, BoundManager, OnCreateBufferModel));
            MaterialComp = AddComponent(new MaterialComponent(this, RenderCore as IMaterialRenderParams, OnCreateMaterial));
            RasterComp = AddComponent(new RasterStateComponent(RenderCore as IRasterStateParam));
            IsTransparentComp = AddComponent(new IsTransparentComponent(this));
            PostEffectComp = AddComponent(new PostEffectComponent(this));
            ShadowComp = AddComponent(new ShadowComponent(RenderCore));
            InvertNormalComp = AddComponent(new InvertNormalComponent(RenderCore as IInvertNormal));
            WireframeComp = AddComponent(new RenderWireframeComponent(RenderCore as IMeshRenderParams));
        }

        public MeshNode(IList<EntityComponent> components)
        {
            foreach(var comp in components)
            {
                AddComponent(comp);
            }
            GeometryComp = Get<GeometryComponent>();
            MaterialComp = Get<MaterialComponent>();
            RasterComp = Get<RasterStateComponent>();
            IsTransparentComp = Get<IsTransparentComponent>();
            PostEffectComp = Get<PostEffectComponent>();
            ShadowComp = Get<ShadowComponent>();
            InvertNormalComp = Get<InvertNormalComponent>();
            WireframeComp = Get<RenderWireframeComponent>();
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new MeshRenderCore();
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected virtual IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return geometry != null && geometry.IsDynamic ? EffectsManager.GeometryBufferManager.Register<DynamicMeshGeometryBufferModel>(modelGuid, geometry)
                : EffectsManager.GeometryBufferManager.Register<DefaultMeshGeometryBufferModel>(modelGuid, geometry);
        }


        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        private bool OnCheckGeometry(Geometry3D geometry)
        {
            return geometry is MeshGeometry3D && !(geometry.Positions == null || geometry.Positions.Count == 0 || geometry.Indices == null || geometry.Indices.Count == 0);
        }

        private MaterialVariable OnCreateMaterial(MaterialCore material)
        {
            return EffectsManager.MaterialVariableManager.Register(material, EffectTechnique);
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray rayWS, ref List<HitTestResult> hits)
        {
            return (GeometryComp.Geometry as MeshGeometry3D).HitTest(context, totalModelMatrix, ref rayWS, ref hits, this.WrapperSource);
        }
    }
}