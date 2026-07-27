using ArenaDefender.Game.Managers;

namespace ArenaDefender.Tests;

/// <summary>Tests for the score/experience-driven leveling system.</summary>
public class ExperienceManagerTests
{
    [Test]
    public void AddExperience_BelowThreshold_DoesNotLevelUp()
    {
        var experienceManager = new ExperienceManager();

        experienceManager.AddExperience(50);

        Assert.That(experienceManager.Level, Is.EqualTo(1));
        Assert.That(experienceManager.LevelUpReady, Is.False);
    }

    [Test]
    public void AddExperience_ReachingThreshold_TriggersLevelUp()
    {
        var experienceManager = new ExperienceManager();

        experienceManager.AddExperience(100);

        Assert.That(experienceManager.Level, Is.EqualTo(2));
        Assert.That(experienceManager.LevelUpReady, Is.True);
    }

    [Test]
    public void AddExperience_CarriesOverRemainderPastThreshold()
    {
        var experienceManager = new ExperienceManager();

        experienceManager.AddExperience(110);

        Assert.That(experienceManager.Experience, Is.EqualTo(10));
    }

    [Test]
    public void FinishLevelUp_ClearsLevelUpReadyFlag()
    {
        var experienceManager = new ExperienceManager();
        experienceManager.AddExperience(100);

        experienceManager.FinishLevelUp();

        Assert.That(experienceManager.LevelUpReady, Is.False);
    }

    [Test]
    public void ExperienceToNextLevel_IncreasesAfterEachLevelUp()
    {
        var experienceManager = new ExperienceManager();
        int firstThreshold = experienceManager.ExperienceToNextLevel;

        experienceManager.AddExperience(firstThreshold);

        Assert.That(experienceManager.ExperienceToNextLevel, Is.GreaterThan(firstThreshold));
    }
}
