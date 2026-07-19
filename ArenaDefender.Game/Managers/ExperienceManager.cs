namespace ArenaDefender.Game.Managers;

public class ExperienceManager
{
    public int Level { get; private set; } = 1;

    public int Experience { get; private set; } = 0;

    public int ExperienceToNextLevel { get; private set; } = 100;

    public bool LevelUpReady { get; private set; }

    public void AddExperience(int amount)
    {
        Experience += amount;

        if (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;

            Level++;

            ExperienceToNextLevel += 50;

            LevelUpReady = true;
        }
    }

    public void FinishLevelUp()
    {
        LevelUpReady = false;
    }
}