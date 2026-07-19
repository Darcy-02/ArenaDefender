using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArenaDefender.Game.Entities;

public class Projectile
{
    public Queue<Vector2> Trail { get; } = new();
    public Vector2 Position { get; private set; }

    public Vector2 Direction { get; }

    public float Speed { get; } = 500f;

    public int Damage { get; } = 25;

    public Projectile(Vector2 position, Vector2 direction)
    {
        Position = position;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
        }
        Direction = direction;
    }

    public void Update(GameTime gameTime)
    {
        Trail.Enqueue(Position);
        if (Trail.Count > 8)
        {
            Trail.Dequeue();
        }
        Position += Direction *
                    Speed *
                    (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public Rectangle Bounds =>
        new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            10,
            10);
}