using Microsoft.Xna.Framework;

namespace ArenaDefender.Game.Core;

/// <summary>
/// Small collection of pure, side-effect free math functions used throughout
/// Arena Defender's gameplay systems. Kept separate from MonoGame's
/// Update/Draw loop so that every function here can be unit tested without
/// spinning up a graphics device.
/// </summary>
public static class MathHelpers
{
    /// <summary>
    /// Straight-line distance between two points. Used for detection radii,
    /// pickup ranges and "find nearest enemy" style queries.
    /// </summary>
    public static float Distance(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b);
    }

    /// <summary>
    /// Returns true when <paramref name="target"/> is within
    /// <paramref name="range"/> units of <paramref name="origin"/>.
    /// </summary>
    public static bool IsWithinRange(Vector2 origin, Vector2 target, float range)
    {
        return Distance(origin, target) <= range;
    }

    /// <summary>
    /// 2D dot product of two vectors. Positive when the vectors point in
    /// broadly the same direction, negative when they point away from each
    /// other, and zero when they are perpendicular.
    /// </summary>
    public static float Dot(Vector2 a, Vector2 b)
    {
        return Vector2.Dot(a, b);
    }

    /// <summary>
    /// Determines whether <paramref name="target"/> lies within a facing
    /// cone in front of <paramref name="facing"/>, using the dot product of
    /// the two normalized directions. Used for enemy "backstab" detection
    /// and could equally drive field-of-view / aim-assist checks.
    /// </summary>
    /// <param name="facing">Normalized facing direction.</param>
    /// <param name="toTarget">Normalized direction from the facer to the target.</param>
    /// <param name="minimumDot">
    /// Cosine threshold. 1 = dead ahead only, 0 = a 180 degree cone in front,
    /// -1 = any direction at all.
    /// </param>
    public static bool IsFacingTarget(Vector2 facing, Vector2 toTarget, float minimumDot)
    {
        if (facing == Vector2.Zero || toTarget == Vector2.Zero)
        {
            return false;
        }

        return Dot(facing, toTarget) >= minimumDot;
    }

    /// <summary>
    /// 2D "cross product" (really the z-component of the 3D cross product
    /// of two vectors lying in the XY plane). A positive result means
    /// <paramref name="b"/> is to the left of <paramref name="a"/>; negative
    /// means it is to the right. Used to decide which way an enemy should
    /// turn to face its target.
    /// </summary>
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    /// <summary>
    /// Returns -1, 0 or 1 depending on whether <paramref name="target"/> is
    /// to the left, directly ahead/behind, or to the right of
    /// <paramref name="facing"/>.
    /// </summary>
    public static int TurnDirection(Vector2 facing, Vector2 target)
    {
        float cross = Cross(facing, target);

        // Note: screen coordinates have Y pointing down (not up like a
        // standard math graph), which flips the usual "positive cross
        // product = counter-clockwise" convention. In this Y-down space,
        // a positive cross product means the target is to the right.
        if (cross > 0f) return 1;   // target is to the right -> turn right
        if (cross < 0f) return -1;  // target is to the left -> turn left
        return 0;
    }

    /// <summary>
    /// Linear interpolation between two floats. <paramref name="t"/> is
    /// clamped to [0, 1] so callers can't accidentally overshoot.
    /// </summary>
    public static float Lerp(float start, float end, float t)
    {
        t = MathHelper.Clamp(t, 0f, 1f);
        return start + (end - start) * t;
    }

    /// <summary>
    /// Linear interpolation between two 2D vectors. <paramref name="t"/> is
    /// clamped to [0, 1].
    /// </summary>
    public static Vector2 Lerp(Vector2 start, Vector2 end, float t)
    {
        t = MathHelper.Clamp(t, 0f, 1f);
        return start + (end - start) * t;
    }

    /// <summary>
    /// Smoothly rotates a direction vector towards a target direction over
    /// time, turning at most <paramref name="maxRadiansDelta"/> radians this
    /// frame. Uses <see cref="Cross(Vector2, Vector2)"/> to pick a turn
    /// direction and <see cref="Lerp(float, float, float)"/> to ease the
    /// angle change, rather than snapping instantly.
    /// </summary>
    public static Vector2 RotateTowards(Vector2 current, Vector2 target, float maxRadiansDelta)
    {
        if (current == Vector2.Zero) return target;
        if (target == Vector2.Zero) return current;

        current.Normalize();
        target.Normalize();

        float currentAngle = (float)System.Math.Atan2(current.Y, current.X);
        float targetAngle = (float)System.Math.Atan2(target.Y, target.X);

        float delta = MathHelper.WrapAngle(targetAngle - currentAngle);

        // Ease the turn using a lerp towards the full angle delta rather
        // than snapping straight to it, then clamp so the enemy never
        // turns faster than its maximum turn speed for this frame.
        float easedDelta = Lerp(0f, delta, 0.5f);
        float step = MathHelper.Clamp(easedDelta, -maxRadiansDelta, maxRadiansDelta);
        float newAngle = currentAngle + step;

        return new Vector2((float)System.Math.Cos(newAngle), (float)System.Math.Sin(newAngle));
    }

    /// <summary>
    /// Clamps a value between a minimum and maximum (inclusive).
    /// </summary>
    public static float Clamp(float value, float min, float max)
    {
        return MathHelper.Clamp(value, min, max);
    }

    /// <summary>
    /// Scales a base value upward by a percentage per wave/level, used for
    /// difficulty scaling (enemy health, damage, spawn rate, etc.).
    /// </summary>
    public static float ScaleByWave(float baseValue, int wave, float percentPerWave)
    {
        if (wave < 1) wave = 1;
        return baseValue * (1f + (wave - 1) * percentPerWave);
    }
}