using Microsoft.Xna.Framework;
using System;

namespace ArenaDefender.Game.Effects;

public class CameraShake
{
    private readonly Random _random = new();

    private float _duration;
    private float _strength;

    public Vector2 Offset { get; private set; }

    public void Shake(float strength, float duration)
    {
        _strength = strength;
        _duration = duration;
    }

    public void Update(GameTime gameTime)
    {
        if (_duration <= 0)
        {
            Offset = Vector2.Zero;
            return;
        }

        _duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        Offset = new Vector2(
            (_random.NextSingle() * 2f - 1f) * _strength,
            (_random.NextSingle() * 2f - 1f) * _strength);
    }
}