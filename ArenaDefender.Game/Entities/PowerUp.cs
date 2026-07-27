using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Entities;

/// <summary>The different effects a power-up pickup can grant.</summary>
public enum PowerUpType
{
    Health,
    SpeedBoost,
    Shield,
    DamageBoost
}

/// <summary>
/// A collectible dropped into the arena that the player can walk into to
/// receive a temporary or permanent buff. This is what the collision
/// system's "player and collectibles/power-ups" requirement checks against
/// - unlike the level-up menu, this is a physical object with its own
/// position and bounds sitting in the world.
/// </summary>
public class PowerUp
{
    public Vector2 Position { get; }

    public PowerUpType Type { get; }

    /// <summary>Seconds remaining before the pickup despawns if uncollected.</summary>
    public float TimeToLive { get; private set; }

    public bool Expired => TimeToLive <= 0f;

    public PowerUp(Vector2 position, PowerUpType type, float timeToLive = 12f)
    {
        Position = position;
        Type = type;
        TimeToLive = timeToLive;
    }

    public void Update(GameTime gameTime)
    {
        TimeToLive -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public Rectangle Bounds =>
        new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            26,
            26);
}
