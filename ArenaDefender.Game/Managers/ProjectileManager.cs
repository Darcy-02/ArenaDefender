using System.Collections.Generic;
using ArenaDefender.Game.Entities;
using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Managers;

public class ProjectileManager
{
    private readonly List<Projectile> _projectiles;

    public IReadOnlyList<Projectile> Projectiles => _projectiles;

    public ProjectileManager()
    {
        _projectiles = new List<Projectile>();
    }

    public void AddProjectile(Projectile projectile)
    {
        _projectiles.Add(projectile);
    }

    public void Update(GameTime gameTime)
    {
        
        foreach (Projectile projectile in _projectiles)
        {
            projectile.Update(gameTime);
        }
    }
}