using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Entities;

public class TankEnemy : Enemy
{
    public TankEnemy(Vector2 position)
        : base(position)
    {
        Speed = 60f;
        Health = 150;
        Damage = 20;
    }
}