using ArenaDefender.Game.Entities;
using ArenaDefender.Game.Managers;
using Microsoft.Xna.Framework;

namespace ArenaDefender.Tests;

/// <summary>
/// Tests for enemy spawn-interval difficulty scaling and per-wave enemy
/// stat scaling.
/// </summary>
public class EnemyTests
{
    [Test]
    public void NextSpawnInterval_DecreasesByStepAmount()
    {
        float result = EnemyManager.NextSpawnInterval(2.0f, step: 0.2f, minimumInterval: 0.5f);

        Assert.That(result, Is.EqualTo(1.8f).Within(0.0001f));
    }

    [Test]
    public void NextSpawnInterval_NeverDropsBelowMinimum()
    {
        float result = EnemyManager.NextSpawnInterval(0.55f, step: 0.2f, minimumInterval: 0.5f);

        Assert.That(result, Is.EqualTo(0.5f));
    }

    [Test]
    public void StandardEnemy_HealthIncreasesWithWave()
    {
        var waveOneEnemy = new StandardEnemy(Vector2.Zero, wave: 1);
        var waveFiveEnemy = new StandardEnemy(Vector2.Zero, wave: 5);

        Assert.That(waveFiveEnemy.Health, Is.GreaterThan(waveOneEnemy.Health));
    }

    [Test]
    public void FastEnemy_IsFasterButWeakerThanTankEnemy()
    {
        var fastEnemy = new FastEnemy(Vector2.Zero);
        var tankEnemy = new TankEnemy(Vector2.Zero);

        Assert.That(fastEnemy.Speed, Is.GreaterThan(tankEnemy.Speed));
        Assert.That(fastEnemy.Health, Is.LessThan(tankEnemy.Health));
    }

    [Test]
    public void TakeDamage_ReducesEnemyHealth()
    {
        var enemy = new StandardEnemy(Vector2.Zero);
        int healthBefore = enemy.Health;

        enemy.TakeDamage(15);

        Assert.That(enemy.Health, Is.EqualTo(healthBefore - 15));
    }

    [Test]
    public void IsDead_WhenHealthDropsToZeroOrBelow_IsTrue()
    {
        var enemy = new FastEnemy(Vector2.Zero); // 25 health at wave 1

        enemy.TakeDamage(100);

        Assert.That(enemy.IsDead, Is.True);
    }
}
