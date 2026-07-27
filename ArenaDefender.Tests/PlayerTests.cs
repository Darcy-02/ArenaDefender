using ArenaDefender.Game.Entities;
using Microsoft.Xna.Framework;

namespace ArenaDefender.Tests;

/// <summary>
/// Tests for player health, damage, flanking (dot product) and power-up
/// buff logic.
/// </summary>
public class PlayerTests
{
    [Test]
    public void TakeDamage_ReducesHealthByDamageAmount()
    {
        var player = new Player(Vector2.Zero);

        player.TakeDamage(20);

        Assert.That(player.Health, Is.EqualTo(80));
    }

    [Test]
    public void TakeDamage_FromBehind_DealsExtraFlankDamage()
    {
        var player = new Player(new Vector2(100, 100));
        // Default facing is "up" (0, -1). An attacker directly below the
        // player is therefore behind them.
        var attackerBehindPosition = new Vector2(100, 200);

        int damageDealt = player.TakeDamage(20, attackerBehindPosition);

        Assert.That(damageDealt, Is.EqualTo(30)); // 20 * 1.5
    }

    [Test]
    public void TakeDamage_FromFront_DealsNormalDamage()
    {
        var player = new Player(new Vector2(100, 100));
        // Attacker above the player, in front of the default "up" facing.
        var attackerInFrontPosition = new Vector2(100, 0);

        int damageDealt = player.TakeDamage(20, attackerInFrontPosition);

        Assert.That(damageDealt, Is.EqualTo(20));
    }

    [Test]
    public void TakeDamage_WhileShieldActive_BlocksAllDamage()
    {
        var player = new Player(Vector2.Zero);
        player.ActivateShield(5f);

        int damageDealt = player.TakeDamage(50);

        Assert.That(damageDealt, Is.EqualTo(0));
        Assert.That(player.Health, Is.EqualTo(100));
    }

    [Test]
    public void IncreaseHealth_CannotExceedMaxHealth()
    {
        var player = new Player(Vector2.Zero);
        player.TakeDamage(10); // Health = 90

        player.IncreaseHealth(50); // Would be 140, should clamp to 100

        Assert.That(player.Health, Is.EqualTo(100));
    }

    [Test]
    public void IncreaseMaxHealth_RaisesBothCapAndCurrentHealth()
    {
        var player = new Player(Vector2.Zero);

        player.IncreaseMaxHealth(25);

        Assert.That(player.MaxHealth, Is.EqualTo(125));
        Assert.That(player.Health, Is.EqualTo(125));
    }

    [Test]
    public void IsDead_WhenHealthReachesZero_IsTrue()
    {
        var player = new Player(Vector2.Zero);

        player.TakeDamage(100);

        Assert.That(player.IsDead, Is.True);
    }

    [Test]
    public void IsDead_WhilePlayerHasHealth_IsFalse()
    {
        var player = new Player(Vector2.Zero);

        player.TakeDamage(10);

        Assert.That(player.IsDead, Is.False);
    }
}