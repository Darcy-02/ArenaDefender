using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArenaDefender.Game.Enums;
using ArenaDefender.Game.Core;
using ArenaDefender.Game.Managers;
using ArenaDefender.Game.Entities;
using System.Collections.Generic;

namespace ArenaDefender.Game;



public class Game1 : Microsoft.Xna.Framework.Game
{
    private readonly GameManager _gameManager;
    private readonly InputManager _inputManager;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private EnemyManager _enemyManager;
    private Texture2D _pixel;
    private ProjectileManager _projectileManager;
    private float _shootTimer;
    private const float ShootInterval = 0.5f;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _gameManager = new GameManager();
        _inputManager = new InputManager();
        _enemyManager = new EnemyManager();
        _projectileManager = new ProjectileManager();
    }

    protected override void Initialize()
    {
        _player = new Player(new Vector2(400, 300));
        _enemyManager.AddEnemy(
        new StandardEnemy(new Vector2(100, 100)));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _inputManager.Update();

        switch (_gameManager.State)
        {
            case GameState.Menu:
                UpdateMenu(gameTime);
                break;

            case GameState.Playing:
                UpdateGameplay(gameTime);
                break;

            case GameState.GameOver:
                UpdateGameOver(gameTime);
                break;
        }

        base.Update(gameTime);
    }
    
    private void UpdateMenu(GameTime gameTime)
    {
        if (_inputManager.IsKeyPressed(Keys.Enter))
        {
            _gameManager.StartGame();
        }
    }

    private void UpdateGameplay(GameTime gameTime)
    {
        _player.Update(gameTime, _inputManager);
        _enemyManager.Update(gameTime, _player.Position);
        _projectileManager.Update(gameTime);
        HandleCollisions();
        _shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_shootTimer >= ShootInterval)
        {
            _shootTimer = 0f;
            ShootNearestEnemy();
        }

    }

    private void UpdateGameOver(GameTime gameTime)
    {

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        if (_gameManager.State == GameState.Menu)
        {
            
        }
        else if (_gameManager.State == GameState.Playing)
        {
            _spriteBatch.Draw(
                _pixel,
                new Rectangle(
                    (int)_player.Position.X,
                    (int)_player.Position.Y,
                    40,
                    40),
                Color.Blue);
                foreach (Enemy enemy in _enemyManager.Enemies)
                {
                    _spriteBatch.Draw(
                        _pixel,
                        new Rectangle(
                            (int)enemy.Position.X,
                            (int)enemy.Position.Y,
                            40,
                            40),
                        Color.Red);
                        foreach (Projectile projectile in _projectileManager.Projectiles)
                        {
                            _spriteBatch.Draw(
                                _pixel,
                                new Rectangle(
                                    (int)projectile.Position.X,
                                    (int)projectile.Position.Y,
                                    10,
                                    10),
                                Color.Yellow);
                        }
                }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void ShootNearestEnemy()
    {
        Enemy? nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Enemy enemy in _enemyManager.Enemies)
        {
            float distance = Vector2.Distance(
                _player.Position,
                enemy.Position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy == null)
        {
            return;
        }

        Vector2 direction =
            nearestEnemy.Position - _player.Position;

        _projectileManager.AddProjectile(
            new Projectile(
                _player.Position,
                direction));
    }
    private void HandleCollisions()
    {
        List<Enemy> deadEnemies = new();
        List<Projectile> usedProjectiles = new();

        foreach (Projectile projectile in _projectileManager.Projectiles)
        {
            foreach (Enemy enemy in _enemyManager.Enemies)
            {
                if (projectile.Bounds.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(projectile.Damage);

                    usedProjectiles.Add(projectile);

                    if (enemy.IsDead)
                    {
                        deadEnemies.Add(enemy);
                    }

                    break;
                }
            }
        }

        foreach (Projectile projectile in usedProjectiles)
        {
            _projectileManager.RemoveProjectile(projectile);
        }

        foreach (Enemy enemy in deadEnemies)
        {
            _enemyManager.RemoveEnemy(enemy);
        }
    }
}
