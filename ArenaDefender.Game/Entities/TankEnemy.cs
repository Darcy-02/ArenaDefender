using Microsoft.Xna.Framework;
using ArenaDefender.Game.Core;

namespace ArenaDefender.Game.Entities;

/// <summary>Slow but heavily armored, high-damage enemy.</summary>
public class TankEnemy : Enemy
{
    protected override float TurnSpeed => MathHelper.Pi * 0.75f;

    public TankEnemy(Vector2 position, int wave = 1)
        : base(position)
    {
        Speed = 60f;
        Health = (int)MathHelpers.ScaleByWave(150f, wave, 0.10f);
        Damage = (int)MathHelpers.ScaleByWave(20f, wave, 0.05f);
        ScoreValue = 25;
    }
}
