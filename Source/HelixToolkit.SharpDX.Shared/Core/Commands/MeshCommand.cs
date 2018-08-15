/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    using Render;
    using Shaders;
    using Utilities;

    public interface IRenderCommand
    {
        /// <summary>
        /// Render routine
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        /// <param name="modelParam"></param>
        void Render<T>(RenderContext context, DeviceContextProxy deviceContext, ref T modelParam) where T : struct;
    }

    public static class DrawCommands
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawIndexed(DeviceContextProxy context, IElementsBufferProxy indexBuffer, IElementsBufferModel instanceModel)
        {
            if (instanceModel == null || !instanceModel.HasElements)
            {
                context.DrawIndexed(indexBuffer.ElementCount, 0, 0);
            }
            else
            {
                context.DrawIndexedInstanced(indexBuffer.ElementCount, instanceModel.Buffer.ElementCount, 0, 0, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawPoints(DeviceContextProxy context, IElementsBufferProxy vertexBuffer, IElementsBufferModel instanceModel)
        {
            if (instanceModel == null || !instanceModel.HasElements)
            {
                context.Draw(vertexBuffer.ElementCount, 0);
            }
            else
            {
                context.DrawInstanced(vertexBuffer.ElementCount, instanceModel.Buffer.ElementCount, 0, 0);
            }
        }
    }

    public struct MeshCommand : IRenderCommand
    {
        public IAttachableBufferModel MeshBuffer;
        public IElementsBufferModel InstanceBuffer;
        public MaterialVariable MaterialVariables;
        public RasterizerStateProxy RasterState;
        public IDynamicReflector DynamicReflector;
        public RenderType RenderType;

        
        public void Render<T>(RenderContext context, DeviceContextProxy deviceContext, ref T modelParam) where T : struct
        {
            int slot = 0;
            if(MeshBuffer.AttachBuffers(deviceContext, ref slot, deviceContext.DeviceResources))
            {
                InstanceBuffer?.AttachBuffer(deviceContext, ref slot);
                ShaderPass pass = MaterialVariables.GetPass(RenderType, context);
                if (pass.IsNULL)
                { return; }
                pass.BindShader(deviceContext);
                pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                deviceContext.SetRasterState(RasterState);
                if (!MaterialVariables.BindMaterial(context, deviceContext, pass))
                {
                    return;
                }
                MaterialVariables.UpdateMaterialStruct(deviceContext, ref modelParam);
                DynamicReflector?.BindCubeMap(deviceContext);
                DrawCommands.DrawIndexed(deviceContext, MeshBuffer.IndexBuffer, InstanceBuffer);
                DynamicReflector?.UnBindCubeMap(deviceContext);
            }
        }
    }

    public struct ShadowCommand : IRenderCommand
    {
        public IAttachableBufferModel MeshBuffer;
        public IElementsBufferModel InstanceBuffer;
        public MaterialVariable MaterialVariables;
        public RasterizerStateProxy RasterState;
        public ShaderPass ShadowPass;


        public void Render<T>(RenderContext context, DeviceContextProxy deviceContext, ref T modelParam) where T : struct
        {
            MaterialVariables.UpdateModelStructOnly(deviceContext, ref modelParam);
            ShadowPass.BindShader(deviceContext);
            ShadowPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            deviceContext.SetRasterState(RasterState);
            DrawCommands.DrawIndexed(deviceContext, MeshBuffer.IndexBuffer, InstanceBuffer);
        }
    }
}
