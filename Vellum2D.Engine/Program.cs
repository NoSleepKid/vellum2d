using Vellum2D.Engine;
using Silk.NET.Maths;
using System;

var engine = new VellumEngine();
engine.SetScene(new TestScene());
engine.Run(1280, 720, "Vellum2D Engine - Smooth Bounds Window");

public class TestScene : GameScene {
    private Sprite _block = null!;
    private float _speed = 0.5f;

    public override void OnInitialize() {
        _block = new Sprite { 
            Position = new Vector2D<float>(0.4f, 0.4f), 
            Size = new Vector2D<float>(0.2f, 0.2f), 
            Layer = 1 
        };
        Spawn(_block);
    }

    public override void OnUpdate(double deltaTime) {
        // CLAMP TIME DELTAS: If the OS freezes our thread during window redraw, 
        // cap the calculation step to a standard 60Hz slice (0.016s) to completely avoid massive position jumps.
        float clampedDelta = Math.Min((float)deltaTime, 0.016f);
        
        var pos = _block.Position;
        pos.X += _speed * clampedDelta;
        
        if (pos.X > 0.7f) {
            pos.X = 0.7f;
            _speed = -_speed;
        }
        else if (pos.X < 0.1f) {
            pos.X = 0.1f;
            _speed = -_speed;
        }
        
        _block.Position = pos;
    }
}
