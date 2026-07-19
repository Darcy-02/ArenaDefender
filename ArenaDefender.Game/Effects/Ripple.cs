using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Effects;

public class Ripple
{
    public Vector2 Position;

    public float Radius;

    public float MaxRadius;

    public float Speed;

    public bool Alive => Radius < MaxRadius;

    public Ripple(
        Vector2 position,
        float maxRadius,
        float speed)
    {
        Position = position;
        Radius = 0;
        MaxRadius = maxRadius;
        Speed = speed;
    }

    public void Update(GameTime gameTime)
    {
        Radius += Speed *
                  (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}