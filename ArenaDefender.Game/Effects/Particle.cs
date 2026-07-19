using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Effects;

public class Particle
{
    public Vector2 Position;
    public Vector2 Velocity;

    public Color Color;

    public float Size;

    public float Life;

    public bool Alive => Life > 0;

    public Particle(
        Vector2 position,
        Vector2 velocity,
        Color color,
        float size,
        float life)
    {
        Position = position;
        Velocity = velocity;
        Color = color;
        Size = size;
        Life = life;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += Velocity * dt;

        Life -= dt;
    }
}