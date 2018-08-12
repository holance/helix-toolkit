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
    public class LineNode : SceneNode, IBoundable
    {
        #region Properties
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            get { return (RenderCore as ILineRenderParams).LineColor; }
            set { (RenderCore as ILineRenderParams).LineColor = value; }
        }
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public float Thickness
        {
            get { return (RenderCore as ILineRenderParams).Thickness; }
            set { (RenderCore as ILineRenderParams).Thickness = value; }
        }

        /// <summary>
        /// Gets or sets the smoothness.
        /// </summary>
        /// <value>
        /// The smoothness.
        /// </value>
        public float Smoothness
        {
            get { return (RenderCore as ILineRenderParams).Smoothness; }
            set { (RenderCore as ILineRenderParams).Smoothness = value; }
        }

        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            set; get;
        } = 1.0;
        #endregion
        public GeometryBoundManager BoundManager { get; }
        public GeometryComponent GeometryComp { get; }
        public ShadowComponent ShadowComp { get; }

        public RasterStateComponent RasterComp { get; }

        public PostEffectComponent PostEffectComp { get; }

        public sealed override BoundingBox OriginalBounds => GeometryComp.BoundManager.OriginalBounds;

        public sealed override BoundingSphere OriginalBoundsSphere => GeometryComp.BoundManager.OriginalBoundsSphere;

        public sealed override BoundingBox Bounds => GeometryComp.BoundManager.Bounds;

        public sealed override BoundingSphere BoundsSphere => GeometryComp.BoundManager.BoundsSphere;

        public sealed override BoundingSphere BoundsSphereWithTransform => GeometryComp.BoundManager.BoundsSphereWithTransform;

        public sealed override BoundingBox BoundsWithTransform => GeometryComp.BoundManager.BoundsWithTransform;

        public LineNode()
        {
            BoundManager = AddComponent(new GeometryBoundManager(this, OnCheckGeometry));
            GeometryComp = AddComponent(new GeometryComponent(this, RenderCore as IGeometryRenderCore, BoundManager, OnCreateBufferModel));
            ShadowComp = AddComponent(new ShadowComponent(RenderCore));
            RasterComp = AddComponent(new RasterStateComponent(RenderCore as IRasterStateParam));
            PostEffectComp = AddComponent(new PostEffectComponent(this));
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected virtual IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return geometry != null && geometry.IsDynamic ? EffectsManager.GeometryBufferManager.Register<DynamicLineGeometryBufferModel>(modelGuid, geometry) 
                : EffectsManager.GeometryBufferManager.Register<DefaultLineGeometryBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new LineRenderCore();
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        ///<para>If<see cref="SceneNode.OnSetRenderTechnique" /> is set, then<see cref="SceneNode.OnSetRenderTechnique" /> instead of<see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.Lines];
        }
        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context))
            {
                return !RenderHost.IsDeferredLighting;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected virtual bool OnCheckGeometry(Geometry3D geometry)
        {
            return geometry is LineGeometry3D && !(geometry.Positions == null || geometry.Positions.Count == 0 || geometry.Indices == null || geometry.Indices.Count == 0);
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
            return (GeometryComp.Geometry as LineGeometry3D).HitTest(context, totalModelMatrix, ref ray, ref hits, this.WrapperSource, (float)HitTestThickness);
        }
    }
}
