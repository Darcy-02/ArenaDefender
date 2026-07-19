using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Entities;

public class FastEnemy : Enemy
{
    public FastEnemy(Vector2 position)
        : base(position)
    {
        Speed = 180f;
        Health = 25;
        Damage = 10;
    }
}