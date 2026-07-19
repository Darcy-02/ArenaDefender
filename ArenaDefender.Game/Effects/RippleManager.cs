using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Effects;

public class RippleManager
{
    private readonly List<Ripple> _ripples = new();

    public IReadOnlyList<Ripple> Ripples => _ripples;

    public void AddRipple(
        Vector2 position,
        float maxRadius = 180f,
        float speed = 350f)
    {
        _ripples.Add(
            new Ripple(
                position,
                maxRadius,
                speed));
    }

    public void Update(GameTime gameTime)
    {
        for (int i = _ripples.Count - 1; i >= 0; i--)
        {
            _ripples[i].Update(gameTime);

            if (!_ripples[i].Alive)
                _ripples.RemoveAt(i);
        }
    }
}