using Silk.NET.Maths;
using System.Runtime.InteropServices;

namespace Vellum2D.Engine;

// Flatten structures entirely into primitive types to enforce unmanaged direct GPU streaming layout
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct GpuSpriteData {
    public float PositionX;
    public float PositionY;
    public float SizeX;
    public float SizeY;
    public int Layer;
    public int TextureID;
}

public class Sprite {
    public Vector2D<float> Position { get; set; } = Vector2D<float>.Zero;
    public Vector2D<float> Size { get; set; } = new Vector2D<float>(0.1f, 0.1f);
    public int Layer { get; set; } = 0;
    public int TextureID { get; set; } = 0;

    public GpuSpriteData ToGpuData() => new GpuSpriteData {
        PositionX = Position.X,
        PositionY = Position.Y,
        SizeX = Size.X,
        SizeY = Size.Y,
        Layer = Layer,
        TextureID = TextureID
    };
}
