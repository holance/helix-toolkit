﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Collections.Generic;
using Matrix = System.Numerics.Matrix4x4;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public class NodePostEffectXRay : SceneNode
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public string EffectName
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).EffectName = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).EffectName;
            }
        }
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).Color = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).Color;
            }
        }
        /// <summary>
        /// Gets or sets the outline fading factor.
        /// </summary>
        /// <value>
        /// The outline fading factor.
        /// </value>
        public float OutlineFadingFactor
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).OutlineFadingFactor = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).OutlineFadingFactor;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable double pass].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable double pass]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDoublePass
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).DoublePass = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).DoublePass;
            }
        } 
        #endregion

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PostEffectMeshXRayCore();
        }

        public sealed override bool HitTest(RenderContext context, Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }

        protected sealed override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
