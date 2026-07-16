using ArenaDefender.Game.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArenaDefender.Game.Managers;

public class EnemyManager
{
    private readonly List<Enemy> _enemies;

    public IReadOnlyList<Enemy> Enemies => _enemies;

    private float _spawnTimer;
    private float _spawnInterval = 2f;

    public EnemyManager()
    {
        _enemies = new List<Enemy>();
    }
    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void Update(GameTime gameTime, Vector2 playerPosition)
    {
        _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0f;

            AddEnemy(new StandardEnemy(new Vector2(50, 50)));
}
        foreach (Enemy enemy in _enemies)
        {
            enemy.Update(gameTime, playerPosition);
        }
    }
}