using ArenaDefender.Game.Core;
using ArenaDefender.Game.Enums;

namespace ArenaDefender.Tests;

/// <summary>Tests for top-level game state transitions and scoring.</summary>
public class GameManagerTests
{
    [Test]
    public void NewGameManager_StartsInMenuState()
    {
        var gameManager = new GameManager();

        Assert.That(gameManager.State, Is.EqualTo(GameState.Menu));
    }

    [Test]
    public void StartGame_TransitionsToPlayingState()
    {
        var gameManager = new GameManager();

        gameManager.StartGame();

        Assert.That(gameManager.State, Is.EqualTo(GameState.Playing));
    }

    [Test]
    public void AddScore_AccumulatesAcrossMultipleCalls()
    {
        var gameManager = new GameManager();

        gameManager.AddScore(10);
        gameManager.AddScore(15);

        Assert.That(gameManager.Score, Is.EqualTo(25));
    }

    [Test]
    public void GameOver_TransitionsToGameOverState()
    {
        var gameManager = new GameManager();
        gameManager.StartGame();

        gameManager.GameOver();

        Assert.That(gameManager.State, Is.EqualTo(GameState.GameOver));
    }

    [Test]
    public void Reset_ClearsScoreAndReturnsToMenu()
    {
        var gameManager = new GameManager();
        gameManager.StartGame();
        gameManager.AddScore(50);
        gameManager.GameOver();

        gameManager.Reset();

        Assert.That(gameManager.State, Is.EqualTo(GameState.Menu));
        Assert.That(gameManager.Score, Is.EqualTo(0));
    }
}
