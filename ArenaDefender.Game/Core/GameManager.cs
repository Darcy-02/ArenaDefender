using ArenaDefender.Game.Enums;

namespace ArenaDefender.Game.Core;

/// <summary>
/// Owns the single source of truth for which <see cref="GameState"/> the
/// game is in and the player's running score, decoupled from both the
/// MonoGame update/draw loop and the UI that displays them.
/// </summary>
public class GameManager
{
    public GameState State { get; private set; }

    public int Score { get; private set; }

    public int Wave { get; private set; }

    public GameManager()
    {
        Reset();
    }

    /// <summary>Moves from the menu (or a finished game) into active play.</summary>
    public void StartGame()
    {
        State = GameState.Playing;
    }

    /// <summary>Pauses gameplay to show the level-up choice screen.</summary>
    public void LevelUp()
    {
        State = GameState.LevelUp;
    }

    /// <summary>Ends the run and shows the game-over screen.</summary>
    public void GameOver()
    {
        State = GameState.GameOver;
    }

    /// <summary>Adds points to the running score (e.g. for a kill or pickup).</summary>
    public void AddScore(int points)
    {
        Score += points;
    }

    public void NextWave()
    {
        Wave++;
    }

    /// <summary>Returns to the menu with score and wave cleared for a fresh run.</summary>
    public void Reset()
    {
        State = GameState.Menu;
        Score = 0;
        Wave = 1;
    }
}