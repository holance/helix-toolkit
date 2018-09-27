/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    /// <summary>
    /// 
    /// </summary>
    public interface IMaterial : INotifyPropertyChanged
    {
        string Name { set; get; }
        Guid Guid { get; }
        MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMaterialVariablePool
    {
        int Count { get; }
        MaterialVariable Register(IMaterial material, IRenderTechnique technique);
    }
}
