﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System;
    using System.Collections.Generic;
    using Utilities;
#if !NETFX_CORE
    [Serializable]
#endif
    public class PointGeometry3D : Geometry3D
    {
        public IEnumerable<Point> Points
        {
            get
            {
                for (int i = 0; i < Positions.Count; ++i)
                {
                    yield return new Point { P0 = Positions[i] };
                }
            }
        }

        protected override IOctreeBasic CreateOctree(OctreeBuildParameter parameter)
        {

            return new StaticPointGeometryOctree(Positions, parameter);
        }

        protected override bool CanCreateOctree()
        {
            return Positions != null && Positions.Count > 0;
        }

        public virtual bool HitTest(RenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits, object originalSource, float hitThickness)
        {
            if(Positions==null || Positions.Count == 0)
            { return false; }
            if (Octree != null)
            {
                return Octree.HitTest(context, originalSource, this, modelMatrix, rayWS, ref hits, hitThickness);
            }
            else
            {
                var svpm = context.ScreenViewProjectionMatrix;
                var smvpm = modelMatrix * svpm;

                var clickPoint3 = rayWS.Position + rayWS.Direction;
                Vector3Helper.TransformCoordinate(ref clickPoint3, ref svpm, out var clickPoint);

                var result = new HitTestResult { IsValid = false, Distance = double.MaxValue };
                var maxDist = hitThickness;
                var lastDist = double.MaxValue;
                var index = 0;

                foreach (var point in Positions)
                {
                    var p0 = Vector3Helper.TransformCoordinate(point, smvpm);
                    var pv = p0 - clickPoint;
                    var dist = pv.Length();
                    if (dist < lastDist && dist <= maxDist)
                    {
                        lastDist = dist;
                        
                        var lp0 = point;
                        Vector3Helper.TransformCoordinate(ref lp0, ref modelMatrix, out var pvv);
                        result.Distance = (rayWS.Position - pvv).Length();
                        result.PointHit = pvv;
                        result.ModelHit = originalSource;
                        result.IsValid = true;
                        result.Tag = index;
                        result.Geometry = this;
                    }

                    index++;
                }

                if (result.IsValid)
                {
                    hits.Add(result);
                }

                return result.IsValid;
            }
        }
    }
}
