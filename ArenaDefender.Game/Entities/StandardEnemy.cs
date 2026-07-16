using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Entities;

public class StandardEnemy : Enemy
{
    public StandardEnemy(Vector2 position)
        : base(position)
    {
        Speed = 100f;
        Health = 50;
        Damage = 10;
    }
}