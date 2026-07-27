using System;
using System.Collections.Generic;
using ArenaDefender.Game.Entities;
using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Managers;

/// <summary>
/// Periodically spawns collectible <see cref="PowerUp"/> pickups at random
/// points in the arena and removes them once collected or expired.
/// </summary>
public class PowerUpManager
{
    private readonly List<PowerUp> _powerUps = new();
    private readonly Random _random = new();

    private float _spawnTimer;
    private const float SpawnInterval = 8f;
    private const int ArenaWidth = 800;
    private const int ArenaHeight = 600;

    public IReadOnlyList<PowerUp> PowerUps => _powerUps;

    public void Update(GameTime gameTime)
    {
        _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_spawnTimer >= SpawnInterval)
        {
            _spawnTimer = 0f;
            SpawnRandomPowerUp();
        }

        for (int i = _powerUps.Count - 1; i >= 0; i--)
        {
            _powerUps[i].Update(gameTime);

            if (_powerUps[i].Expired)
            {
                _powerUps.RemoveAt(i);
            }
        }
    }

    private void SpawnRandomPowerUp()
    {
        var position = new Vector2(
            _random.Next(40, ArenaWidth - 40),
            _random.Next(40, ArenaHeight - 40));

        var values = (PowerUpType[])Enum.GetValues(typeof(PowerUpType));
        var type = values[_random.Next(values.Length)];

        _powerUps.Add(new PowerUp(position, type));
    }

    public void Remove(PowerUp powerUp)
    {
        _powerUps.Remove(powerUp);
    }
}
