using Microsoft.Xna.Framework;
using ArenaDefender.Game.Managers;
using Microsoft.Xna.Framework.Input;
using ArenaDefender.Game.Core;

namespace ArenaDefender.Game.Entities;

/// <summary>
/// The player-controlled character: movement, health, facing direction and
/// the temporary buffs applied by power-ups.
/// </summary>
public class Player
{
    public Vector2 Position { get; private set; }

    public float Speed { get; private set; }

    public int MaxHealth { get; private set; } = 100;

    public int Health { get; private set; } = 100;

    /// <summary>
    /// Displayed health lerps towards <see cref="Health"/> each frame so the
    /// health bar animates smoothly instead of snapping instantly on damage.
    /// </summary>
    public float DisplayedHealth { get; private set; } = 100f;

    /// <summary>
    /// Normalized direction the player last moved in. Defaults to "up".
    /// Used for facing-dependent gameplay checks (e.g. being flanked).
    /// </summary>
    public Vector2 Facing { get; private set; } = new Vector2(0, -1);

    /// <summary>True while a temporary shield power-up is active.</summary>
    public bool ShieldActive { get; private set; }

    private float _shieldTimeRemaining;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        Speed = 250f;
        Health = 100;
        DisplayedHealth = 100f;
    }

    public void Update(GameTime gameTime, InputManager input)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 direction = Vector2.Zero;

        if (input.IsKeyDown(Keys.W))
            direction.Y--;
            //(0,-1)

        if (input.IsKeyDown(Keys.S))
            direction.Y++;
            //(0,1)

        if (input.IsKeyDown(Keys.A))
            direction.X--;
            //(-1,0)

        if (input.IsKeyDown(Keys.D))
            direction.X++;
            //(1,0)

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            Facing = direction;
        }

        Position += direction * Speed * dt;

        // Smoothly animate the displayed health bar towards the real value.
        DisplayedHealth = MathHelpers.Lerp(DisplayedHealth, Health, dt * 6f);

        if (_shieldTimeRemaining > 0f)
        {
            _shieldTimeRemaining -= dt;
            ShieldActive = _shieldTimeRemaining > 0f;
        }
    }

    public Rectangle Bounds =>
        new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            40,
            40);

    /// <summary>
    /// Applies damage to the player. If <paramref name="attackerPosition"/>
    /// is behind the player relative to their facing direction, the hit is
    /// treated as a "flank" and deals extra damage - a direct gameplay use
    /// of the dot product between facing and attacker direction. A shield
    /// negates all damage while active.
    /// </summary>
    public int TakeDamage(int damage, Vector2? attackerPosition = null)
    {
        if (ShieldActive)
        {
            return 0;
        }

        int finalDamage = damage;

        if (attackerPosition.HasValue)
        {
            Vector2 toAttacker = attackerPosition.Value - Position;

            if (toAttacker != Vector2.Zero)
            {
                toAttacker.Normalize();

                // Dot product < 0 means the attacker is roughly behind the
                // player (opposite of where they're facing) -> flanked.
                bool isFlanked = MathHelpers.Dot(Facing, toAttacker) < -0.25f;

                if (isFlanked)
                {
                    finalDamage = (int)(damage * 1.5f);
                }
            }
        }

        Health -= finalDamage;
        return finalDamage;
    }

    public void IncreaseSpeed(float amount)
    {
        Speed += amount;
    }

    public void IncreaseHealth(int amount)
    {
        Health = System.Math.Min(MaxHealth, Health + amount);
    }

    /// <summary>Raises the health cap and immediately grants the extra health.</summary>
    public void IncreaseMaxHealth(int amount)
    {
        MaxHealth += amount;
        Health += amount;
    }

    /// <summary>Activates (or refreshes) a temporary damage-blocking shield.</summary>
    public void ActivateShield(float durationSeconds)
    {
        _shieldTimeRemaining = durationSeconds;
        ShieldActive = true;
    }

    public bool IsDead => Health <= 0;
}
