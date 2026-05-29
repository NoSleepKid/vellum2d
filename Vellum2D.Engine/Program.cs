using Vellum2D.Engine;
using Silk.NET.Maths;

var engine = new VellumEngine();
engine.SetScene(new TestScene());
engine.Run(1280, 720, "Vellum2D Engine - Pixel Sweep Test");

public class TestScene : GameScene {
    private Sprite _block = null!;
    private float _speed = 0.5f; // Normalized units per second

    public override void OnInitialize() {
        // Positioned dead center initially on Layer 1
        _block = new Sprite { 
            Position = new Vector2D<float>(0.4f, 0.4f), 
            Size = new Vector2D<float>(0.2f, 0.2f), 
            Layer = 1 
        };
        Spawn(_block);
    }

    public override void OnUpdate(double deltaTime) {
        var pos = _block.Position;
        
        // Paced coordinate step updates to prevent your entity from flashing into deep space
        pos.X += _speed * (float)deltaTime;
        
        // Bound checks within our 0.0 to 1.0 viewport space
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
