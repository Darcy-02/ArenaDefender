using Microsoft.Xna.Framework;
using ArenaDefender.Game.Core;

namespace ArenaDefender.Game.Entities;

/// <summary>Balanced, no-frills enemy. Moderate speed, health and damage.</summary>
public class StandardEnemy : Enemy
{
    public StandardEnemy(Vector2 position, int wave = 1)
        : base(position)
    {
        Speed = 100f;
        Health = (int)MathHelpers.ScaleByWave(50f, wave, 0.10f);
        Damage = (int)MathHelpers.ScaleByWave(10f, wave, 0.05f);
        ScoreValue = 10;
    }
}
