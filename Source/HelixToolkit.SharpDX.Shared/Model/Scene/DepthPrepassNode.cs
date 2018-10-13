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
    /// Do a depth prepass before rendering.
    /// <para>Must customize the DefaultEffectsManager and set DepthStencilState to DefaultDepthStencilDescriptions.DSSDepthEqualNoWrite in default ShaderPass from EffectsManager to achieve best performance.</para>
    /// </summary>
    public sealed class DepthPrepassNode : SceneNode
    {
        protected override RenderCore OnCreateRenderCore()
        {
            return new DepthPrepassCore();
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
