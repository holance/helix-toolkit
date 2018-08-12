/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using Components;
    /// <summary>
    /// 
    /// </summary>
    public class PointNode : SceneNode, IBoundable
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
            get { return (RenderCore as IPointRenderParams).PointColor; }
            set { (RenderCore as IPointRenderParams).PointColor = value; }
        }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Size2F Size
        {
            get { return new Size2F((RenderCore as IPointRenderParams).Width, (RenderCore as IPointRenderParams).Height); }
            set
            {
                (RenderCore as IPointRenderParams).Width = value.Width;
                (RenderCore as IPointRenderParams).Height = value.Height;
            }
        }
        /// <summary>
        /// Gets or sets the figure.
        /// </summary>
        /// <value>
        /// The figure.
        /// </value>
        public PointFigure Figure
        {
            get { return (RenderCore as IPointRenderParams).Figure; }
            set { (RenderCore as IPointRenderParams).Figure = value; }
        }
        /// <summary>
        /// Gets or sets the figure ratio.
        /// </summary>
        /// <value>
        /// The figure ratio.
        /// </value>
        public float FigureRatio
        {
            get { return (RenderCore as IPointRenderParams).FigureRatio; }
            set { (RenderCore as IPointRenderParams).FigureRatio = value; }
        }

        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            set; get;
        } = 4;
        public GeometryBoundManager BoundManager { get; }
        public GeometryComponent GeometryComp { get; }
        public ShadowComponent ShadowComp { get; }
        public PostEffectComponent PostEffectComp { get; }
        public RasterStateComponent RasterComp { get; }

        public sealed override BoundingBox OriginalBounds => GeometryComp.BoundManager.OriginalBounds;

        public sealed override BoundingSphere OriginalBoundsSphere => GeometryComp.BoundManager.OriginalBoundsSphere;

        public sealed override BoundingBox Bounds => GeometryComp.BoundManager.Bounds;

        public sealed override BoundingSphere BoundsSphere => GeometryComp.BoundManager.BoundsSphere;

        public sealed override BoundingSphere BoundsSphereWithTransform => GeometryComp.BoundManager.BoundsSphereWithTransform;

        public sealed override BoundingBox BoundsWithTransform => GeometryComp.BoundManager.BoundsWithTransform;
        #endregion

        public PointNode()
        {
            BoundManager = AddComponent(new GeometryBoundManager(this, OnCheckGeometry));
            GeometryComp = AddComponent(new GeometryComponent(this, RenderCore as IGeometryRenderCore, BoundManager, OnCreateBufferModel));
            ShadowComp = AddComponent(new ShadowComponent(RenderCore));
            PostEffectComp = AddComponent(new PostEffectComponent(this));
            RasterComp = AddComponent(new RasterStateComponent(RenderCore as IRasterStateParam));
        }


        /// <summary>
        /// Distances the ray to point.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static double DistanceRayToPoint(Ray r, Vector3 p)
        {
            Vector3 v = r.Direction;
            Vector3 w = p - r.Position;

            float c1 = Vector3.Dot(w, v);
            float c2 = Vector3.Dot(v, v);
            float b = c1 / c2;

            Vector3 pb = r.Position + v * b;
            return (p - pb).Length();
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected virtual IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return geometry != null && geometry.IsDynamic ? EffectsManager.GeometryBufferManager.Register<DynamicPointGeometryBufferModel>(modelGuid, geometry) 
                : EffectsManager.GeometryBufferManager.Register<DefaultPointGeometryBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PointRenderCore();
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
            return host.EffectsManager[DefaultRenderTechniqueNames.Points];
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
            return geometry is PointGeometry3D && !(geometry.Positions == null || geometry.Positions.Count == 0);
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
            return (GeometryComp.Geometry as PointGeometry3D).HitTest(context, totalModelMatrix, ref ray, ref hits, this.WrapperSource, (float)HitTestThickness);
        }
    }
}