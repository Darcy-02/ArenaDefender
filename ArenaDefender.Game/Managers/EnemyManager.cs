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
    private int _wave = 1;
    private float _waveTimer = 0f;
    private const float WaveDuration = 30f;
    private readonly Random _random = new();
    public int Wave => _wave;

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

        Vector2 position = GetRandomSpawnPosition();

        int enemyType = _random.Next(3);

        Enemy enemy = enemyType switch
        {
            0 => new StandardEnemy(position),
            1 => new FastEnemy(position),
            _ => new TankEnemy(position)
        };

        AddEnemy(enemy);
        }
        foreach (Enemy enemy in _enemies)
        {
            enemy.Update(gameTime, playerPosition);
        }

        _waveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_waveTimer >= WaveDuration)
        {
            _waveTimer = 0f;
            _wave++;

            _spawnInterval = Math.Max(0.5f, _spawnInterval - 0.2f);
        }
    }
    private Vector2 GetRandomSpawnPosition()
    {
        int side = _random.Next(4);

        switch (side)
        {
            case 0: 
                return new Vector2(_random.Next(0, 800), 0);

            case 1: 
                return new Vector2(800, _random.Next(0, 600));

            case 2: 
                return new Vector2(_random.Next(0, 800), 600);

            default: 
                return new Vector2(0, _random.Next(0, 600));
        }
    }
    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }
}