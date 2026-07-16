using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Entities;

public abstract class Enemy
{
    public Vector2 Position { get; protected set; }

    public float Speed { get; protected set; }

    public int Health { get; protected set; }

    public int Damage { get; protected set; }

    public int ScoreValue { get; protected set; }

    public bool IsAlive => Health > 0;

    protected Enemy(Vector2 startPosition)
    {
        Position = startPosition;
    }
    public void Update(GameTime gameTime, Vector2 playerPosition)
{
    Vector2 direction = playerPosition - Position;

    if (direction != Vector2.Zero)
    {
        direction.Normalize();
    }

    Position += direction * Speed *
                (float)gameTime.ElapsedGameTime.TotalSeconds;
}
    public Rectangle Bounds =>
    new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        40,
        40);
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
        public bool IsDead => Health <= 0;
}