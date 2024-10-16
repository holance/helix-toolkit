﻿using HelixToolkit.SharpDX.Model.Scene;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IModelContainer : IRenderHost
{
    /// <summary>
    /// 
    /// </summary>
    IEnumerable<SceneNode?> Renderables
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewport"></param>
    void AttachViewport3DX(IViewport3DX viewport);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewport"></param>
    void DettachViewport3DX(IViewport3DX viewport);
    /// <summary>
    /// 
    /// </summary>
    IRenderHost? CurrentRenderHost
    {
        set; get;
    }

    void Attach(IRenderHost host);
    void Detach(IRenderHost host);
}
