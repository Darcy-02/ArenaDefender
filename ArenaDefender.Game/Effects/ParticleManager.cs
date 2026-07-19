using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Effects;

public class ParticleManager
{
    private readonly List<Particle> _particles = new();

    private readonly Random _random = new();

    public IReadOnlyList<Particle> Particles => _particles;

    public void Update(GameTime gameTime)
    {
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            _particles[i].Update(gameTime);

            if (!_particles[i].Alive)
                _particles.RemoveAt(i);
        }
    }

    public void CreateExplosion(Vector2 position, Color color)
    {
        for (int i = 0; i < 20; i++)
        {
            float angle =
                MathHelper.TwoPi * (float)_random.NextDouble();

            float speed =
                50f + 150f * (float)_random.NextDouble();

            Vector2 velocity =
                new Vector2(
                    (float)Math.Cos(angle),
                    (float)Math.Sin(angle))
                * speed;

            _particles.Add(
                new Particle(
                    position,
                    velocity,
                    color,
                    5f,
                    0.5f));
        }
    }
}