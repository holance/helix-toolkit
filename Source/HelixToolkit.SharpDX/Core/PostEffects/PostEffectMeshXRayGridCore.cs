﻿using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class PostEffectMeshXRayGridCore : RenderCore, IPostEffectMeshXRayGrid
{
    #region Variables
    private readonly List<KeyValuePair<SceneNode, IEffectAttributes>> currentCores = new();
    private readonly ConstantBufferComponent modelCB;
    private BorderEffectStruct modelStruct;
    #endregion
    #region Properties
    private string effectName = DefaultRenderTechniqueNames.PostEffectMeshXRayGrid;
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
            SetAffectsCanRenderFlag(ref effectName, value);
        }
        get
        {
            return effectName;
        }
    }

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>
    /// The color of the border.
    /// </value>
    public Color4 Color
    {
        set
        {
            SetAffectsRender(ref modelStruct.Color, value);
        }
        get
        {
            return modelStruct.Color;
        }
    }

    private int gridDensity = 8;
    /// <summary>
    /// Gets or sets the grid density.
    /// </summary>
    /// <value>
    /// The grid density.
    /// </value>
    public int GridDensity
    {
        set
        {
            SetAffectsRender(ref gridDensity, value);
        }
        get
        {
            return gridDensity;
        }
    }

    private float dimmingFactor = 0.8f;
    /// <summary>
    /// Gets or sets the dim factor on original color
    /// </summary>
    /// <value>
    /// The dim factor.
    /// </value>
    public float DimmingFactor
    {
        set
        {
            SetAffectsRender(ref dimmingFactor, value);
        }
        get
        {
            return dimmingFactor;
        }
    }

    private float blendingFactor = 1f;
    /// <summary>
    /// Gets or sets the blending factor for grid and original mesh color blending
    /// </summary>
    /// <value>
    /// The blending factor.
    /// </value>
    public float BlendingFactor
    {
        set
        {
            SetAffectsRender(ref blendingFactor, value);
        }
        get
        {
            return blendingFactor;
        }
    }
    /// <summary>
    /// Gets or sets the name of the x ray drawing pass. This is the final pass to draw mesh and grid overlay onto render target
    /// </summary>
    /// <value>
    /// The name of the x ray drawing pass.
    /// </value>
    public string XRayDrawingPassName
    {
        set; get;
    } = DefaultPassNames.EffectMeshXRayGridP3;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PostEffectMeshXRayGridCore"/> class.
    /// </summary>
    public PostEffectMeshXRayGridCore() : base(RenderType.PostEffect)
    {
        modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes)));
        Color = Maths.Color.Blue;
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        return true;
    }

    protected override void OnDetach()
    {
    }

    protected override bool OnUpdateCanRenderFlag()
    {
        return IsAttached && !string.IsNullOrEmpty(EffectName);
    }
    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="deviceContext">The device context.</param>
    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        var buffer = context.RenderHost.RenderBuffer;
        if (buffer is null)
        {
            return;
        }
        var depthStencilBuffer = buffer.DepthStencilBufferNoMSAA;
        deviceContext.SetRenderTarget(depthStencilBuffer, buffer.FullResPPBuffer?.CurrentRTV);
        var viewport = context.Viewport;
        deviceContext.SetViewport(ref viewport);
        deviceContext.SetScissorRectangle(ref viewport);
        //First pass, draw onto stencil buffer
        for (var i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
        {
            var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
            if (mesh is not null && mesh.TryGetPostEffect(EffectName, out var effect))
            {
                currentCores.Add(new KeyValuePair<SceneNode, IEffectAttributes>(mesh, effect));
                context.CustomPassName = DefaultPassNames.EffectMeshXRayGridP1;
                var pass = mesh?.EffectTechnique?[DefaultPassNames.EffectMeshXRayGridP1];
                if (pass is null || pass.IsNULL)
                {
                    continue;
                }
                pass.BindShader(deviceContext);
                pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                mesh!.RenderCustom(context, deviceContext);
            }
        }
        //Second pass, remove not covered part from stencil buffer
        for (var i = 0; i < currentCores.Count; ++i)
        {
            var mesh = currentCores[i].Key;
            context.CustomPassName = DefaultPassNames.EffectMeshXRayGridP2;
            var pass = mesh?.EffectTechnique?[DefaultPassNames.EffectMeshXRayGridP2];
            if (pass is null || pass.IsNULL)
            {
                continue;
            }
            pass.BindShader(deviceContext);
            pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            mesh!.RenderCustom(context, deviceContext);
        }

        OnUpdatePerModelStruct(context);
        modelCB.Upload(deviceContext, ref modelStruct);
        //Thrid pass, draw mesh with grid overlay
        for (var i = 0; i < currentCores.Count; ++i)
        {
            var mesh = currentCores[i].Key;
            var color = Color;
            if (currentCores[i].Value.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out var attribute) && attribute is string colorStr)
            {
                color = colorStr.ToColor4();
            }
            if (modelStruct.Color != color)
            {
                modelStruct.Color = color;
                modelCB.Upload(deviceContext, ref modelStruct);
            }
            context.CustomPassName = XRayDrawingPassName;
            var pass = mesh?.EffectTechnique?[XRayDrawingPassName];
            if (pass is null || pass.IsNULL)
            {
                continue;
            }
            pass.BindShader(deviceContext);
            pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            if (mesh!.RenderCore is IMaterialRenderParams material)
            {
                material.MaterialVariables?.BindMaterialResources(context, deviceContext, pass);
            }
            mesh.RenderCustom(context, deviceContext);
        }
        currentCores.Clear();
    }

    private void OnUpdatePerModelStruct(RenderContext context)
    {
        modelStruct.Param.M11 = gridDensity;
        modelStruct.Param.M12 = dimmingFactor;
        modelStruct.Param.M13 = blendingFactor;
    }
}
