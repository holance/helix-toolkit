﻿using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

public sealed class Sprite2DBufferModel : DisposeObject, IGUID, IAttachableBufferModel
{
    public PrimitiveTopology Topology { get; set; } = PrimitiveTopology.TriangleList;

    public IElementsBufferProxy?[] VertexBuffer { get; } = new DynamicBufferProxy?[1];

    public IEnumerable<int> VertexStructSize
    {
        get
        {
            return VertexBuffer.Select(x => x != null ? x.StructureSize : 0);
        }
    }

    public IElementsBufferProxy? IndexBuffer => indexBuffer;

    public Guid GUID { get; } = Guid.NewGuid();

    public SpriteStruct[]? Sprites
    {
        set; get;
    }
    public int SpriteCount;

    public int[]? Indices
    {
        set; get;
    }
    public int IndexCount
    {
        set; get;
    }

    private DynamicBufferProxy? vertextBuffer;
    private IElementsBufferProxy? indexBuffer;

    public Sprite2DBufferModel()
    {
        vertextBuffer = new DynamicBufferProxy(SpriteStruct.SizeInBytes, BindFlags.VertexBuffer);
        VertexBuffer[0] = vertextBuffer;
        indexBuffer = new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer);
    }

    public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources? deviceResources)
    {
        if (UpdateBuffers(context, deviceResources) && vertextBuffer is not null && IndexBuffer is not null)
        {
            context.SetVertexBuffers(0, new VertexBufferBinding(vertextBuffer.Buffer, vertextBuffer.StructureSize, vertextBuffer.Offset));
            context.SetIndexBuffer(IndexBuffer.Buffer, global::SharpDX.DXGI.Format.R32_UInt, IndexBuffer.Offset);
            return true;
        }
        return false;
    }

    public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources? deviceResources)
    {
        if (vertextBuffer is null || IndexBuffer is null || SpriteCount == 0 || IndexCount == 0 || Sprites == null || Indices == null || Sprites.Length < SpriteCount || Indices.Length < IndexCount)
        {
            return false;
        }
        vertextBuffer.UploadDataToBuffer(context, Sprites, SpriteCount);
        IndexBuffer.UploadDataToBuffer(context, Indices, IndexCount);
        return true;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref vertextBuffer);
        VertexBuffer[0] = null;
        RemoveAndDispose(ref indexBuffer);
        base.OnDispose(disposeManagedResources);
    }
}
