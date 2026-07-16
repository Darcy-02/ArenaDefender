using ArenaDefender.Game.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace ArenaDefender.Game.Managers;

public class EnemyManager
{
    private readonly List<Enemy> _enemies;

    public IReadOnlyList<Enemy> Enemies => _enemies;

    private float _spawnTimer;
    private float _spawnInterval = 2f;
    private readonly Random _random = new();

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

            AddEnemy(new StandardEnemy(GetRandomSpawnPosition()));
        }
        foreach (Enemy enemy in _enemies)
        {
            enemy.Update(gameTime, playerPosition);
        }
    }
    private Vector2 GetRandomSpawnPosition()
    {
        int side = _random.Next(4);

        switch (side)
        {
            case 0: // Top
                return new Vector2(_random.Next(0, 800), 0);

            case 1: // Right
                return new Vector2(800, _random.Next(0, 600));

            case 2: // Bottom
                return new Vector2(_random.Next(0, 800), 600);

            default: // Left
                return new Vector2(0, _random.Next(0, 600));
        }
    }
}