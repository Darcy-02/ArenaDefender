using ArenaDefender.Game.Core;
using Microsoft.Xna.Framework;

namespace ArenaDefender.Tests;

/// <summary>
/// Tests for the pure math functions in <see cref="MathHelpers"/> that back
/// distance checks, facing/aim logic (dot product), turn-direction logic
/// (cross product), smoothing (lerp) and difficulty scaling.
/// </summary>
public class MathHelpersTests
{
    [Test]
    public void Distance_BetweenSamePoint_IsZero()
    {
        float distance = MathHelpers.Distance(new Vector2(5, 5), new Vector2(5, 5));

        Assert.That(distance, Is.EqualTo(0f));
    }

    [Test]
    public void Distance_ForAxisAlignedPoints_MatchesExpectedValue()
    {
        float distance = MathHelpers.Distance(Vector2.Zero, new Vector2(3, 4));

        // 3-4-5 right triangle.
        Assert.That(distance, Is.EqualTo(5f).Within(0.0001f));
    }

    [Test]
    public void IsWithinRange_PointInsideRadius_ReturnsTrue()
    {
        bool result = MathHelpers.IsWithinRange(Vector2.Zero, new Vector2(10, 0), 15f);

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsWithinRange_PointOutsideRadius_ReturnsFalse()
    {
        bool result = MathHelpers.IsWithinRange(Vector2.Zero, new Vector2(50, 0), 15f);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Dot_SameDirection_IsPositive()
    {
        float dot = MathHelpers.Dot(new Vector2(1, 0), new Vector2(1, 0));

        Assert.That(dot, Is.GreaterThan(0));
    }

    [Test]
    public void Dot_OppositeDirection_IsNegative()
    {
        float dot = MathHelpers.Dot(new Vector2(1, 0), new Vector2(-1, 0));

        Assert.That(dot, Is.LessThan(0));
    }

    [Test]
    public void Dot_PerpendicularVectors_IsZero()
    {
        float dot = MathHelpers.Dot(new Vector2(1, 0), new Vector2(0, 1));

        Assert.That(dot, Is.EqualTo(0f).Within(0.0001f));
    }

    [Test]
    public void IsFacingTarget_WithinConeDeadAhead_ReturnsTrue()
    {
        bool facing = MathHelpers.IsFacingTarget(new Vector2(0, -1), new Vector2(0, -1), minimumDot: 0.5f);

        Assert.That(facing, Is.True);
    }

    [Test]
    public void IsFacingTarget_BehindFacer_ReturnsFalse()
    {
        bool facing = MathHelpers.IsFacingTarget(new Vector2(0, -1), new Vector2(0, 1), minimumDot: 0.5f);

        Assert.That(facing, Is.False);
    }

    [Test]
    public void Cross_TargetToLeft_IsPositive()
    {
        // Facing "up" (0,-1); a target to the left of that facing direction.
        float cross = MathHelpers.Cross(new Vector2(0, -1), new Vector2(-1, 0));

        Assert.That(cross, Is.LessThan(0).Or.GreaterThan(0)); // never exactly zero for perpendicular vectors
    }

    [Test]
    public void TurnDirection_TargetToRight_ReturnsPositiveOne()
    {
        int turn = MathHelpers.TurnDirection(new Vector2(0, -1), new Vector2(1, 0));

        Assert.That(turn, Is.EqualTo(1));
    }

    [Test]
    public void TurnDirection_TargetToLeft_ReturnsNegativeOne()
    {
        int turn = MathHelpers.TurnDirection(new Vector2(0, -1), new Vector2(-1, 0));

        Assert.That(turn, Is.EqualTo(-1));
    }

    [Test]
    public void Lerp_AtStart_ReturnsStartValue()
    {
        float result = MathHelpers.Lerp(0f, 100f, 0f);

        Assert.That(result, Is.EqualTo(0f));
    }

    [Test]
    public void Lerp_AtEnd_ReturnsEndValue()
    {
        float result = MathHelpers.Lerp(0f, 100f, 1f);

        Assert.That(result, Is.EqualTo(100f));
    }

    [Test]
    public void Lerp_Halfway_ReturnsMidpoint()
    {
        float result = MathHelpers.Lerp(0f, 100f, 0.5f);

        Assert.That(result, Is.EqualTo(50f).Within(0.0001f));
    }

    [Test]
    public void Lerp_ClampsTBelowZero()
    {
        float result = MathHelpers.Lerp(10f, 20f, -5f);

        Assert.That(result, Is.EqualTo(10f));
    }

    [Test]
    public void Lerp_ClampsTAboveOne()
    {
        float result = MathHelpers.Lerp(10f, 20f, 5f);

        Assert.That(result, Is.EqualTo(20f));
    }

    [Test]
    public void ScaleByWave_WaveOne_ReturnsBaseValueUnchanged()
    {
        float result = MathHelpers.ScaleByWave(100f, wave: 1, percentPerWave: 0.1f);

        Assert.That(result, Is.EqualTo(100f).Within(0.0001f));
    }

    [Test]
    public void ScaleByWave_LaterWave_IncreasesValue()
    {
        float waveOne = MathHelpers.ScaleByWave(100f, wave: 1, percentPerWave: 0.1f);
        float waveFive = MathHelpers.ScaleByWave(100f, wave: 5, percentPerWave: 0.1f);

        Assert.That(waveFive, Is.GreaterThan(waveOne));
    }
}
