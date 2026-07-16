using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Entities;

public class Projectile
{
    public Vector2 Position { get; private set; }

    public Vector2 Direction { get; }

    public float Speed { get; } = 500f;

    public int Damage { get; } = 25;

    public Projectile(Vector2 position, Vector2 direction)
    {
        Position = position;

        if (Direction != Vector2.Zero)
        {
            Direction.Normalize();
        }
        Direction = direction;
    }

    public void Update(GameTime gameTime)
    {
        Position += Direction *
                    Speed *
                    (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}