using Microsoft.Xna.Framework;
using ArenaDefender.Game.Core;

namespace ArenaDefender.Game.Entities;

/// <summary>
/// Base class for every enemy archetype. Handles homing movement towards
/// the player, a smoothly-turning facing direction (cross product + lerp),
/// and shared health/damage/flash-on-hit bookkeeping.
/// </summary>
public abstract class Enemy
{
    public Vector2 Position { get; protected set; }

    public float Speed { get; protected set; }

    public int Health { get; protected set; }

    public int Damage { get; protected set; }

    public int ScoreValue { get; protected set; }

    public bool IsAlive => Health > 0;
    private float _hitFlashTimer;
    public bool IsFlashing => _hitFlashTimer > 0f;

    /// <summary>Normalized direction the enemy currently faces/moves in.</summary>
    public Vector2 Facing { get; protected set; } = new Vector2(0, 1);

    /// <summary>Maximum turn rate, in radians per second.</summary>
    protected virtual float TurnSpeed => MathHelper.Pi * 1.5f;

    protected Enemy(Vector2 startPosition)
    {
        Position = startPosition;
    }
    public void Update(GameTime gameTime, Vector2 playerPosition)
{
    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    if (_hitFlashTimer > 0f)
    {
        _hitFlashTimer -= dt;
    }
    Vector2 direction = playerPosition - Position;

    if (direction != Vector2.Zero)
    {
        direction.Normalize();

        // Use the cross product (to know which way to turn) combined with
        // a lerp-eased rotation so enemies steer smoothly towards the
        // player instead of snapping straight at them every frame.
        Facing = MathHelpers.RotateTowards(Facing, direction, TurnSpeed * dt);
    }

    Position += direction * Speed * dt;
}
    public Rectangle Bounds =>
    new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        40,
        40);

    /// <summary>Applies damage and triggers a brief hit-flash effect.</summary>
    public void TakeDamage(int damage)
    {
        Health -= damage;
        _hitFlashTimer = 0.1f;
    }

    public bool IsDead => Health <= 0;
}