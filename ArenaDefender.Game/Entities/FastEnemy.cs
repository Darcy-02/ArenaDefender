using Microsoft.Xna.Framework;
using ArenaDefender.Game.Core;

namespace ArenaDefender.Game.Entities;

/// <summary>Fast but fragile enemy that closes distance quickly.</summary>
public class FastEnemy : Enemy
{
    protected override float TurnSpeed => MathHelper.Pi * 2.5f;

    public FastEnemy(Vector2 position, int wave = 1)
        : base(position)
    {
        Speed = 180f;
        Health = (int)MathHelpers.ScaleByWave(25f, wave, 0.10f);
        Damage = (int)MathHelpers.ScaleByWave(10f, wave, 0.05f);
        ScoreValue = 15;
    }
}
