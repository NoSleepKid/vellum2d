using System.Collections.Generic;
namespace Vellum2D.Engine;
public abstract class GameScene {
    public List<Sprite> Sprites { get; private set; } = new List<Sprite>();
    public abstract void OnInitialize();
    public abstract void OnUpdate(double deltaTime);
    public void Spawn(Sprite sprite) => Sprites.Add(sprite);
}
