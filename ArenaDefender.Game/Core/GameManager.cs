using ArenaDefender.Game.Enums;

namespace ArenaDefender.Game.Core;

public class GameManager
{
    public GameState State { get; private set; }

    public int Score { get; private set; }

    public int Wave { get; private set; }

    public GameManager()
    {
        Reset();
    }

    public void StartGame()
    {
        State = GameState.Playing;
    }

    public void LevelUp()
    {
        State = GameState.LevelUp;
    }

    public void GameOver()
    {
        State = GameState.GameOver;
    }

    public void AddScore(int points)
    {
        Score += points;
    }

    public void NextWave()
    {
        Wave++;
    }

    public void Reset()
    {
        State = GameState.Menu;
        Score = 0;
        Wave = 1;
    }
}