using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using ArenaDefender.Game.Enums;
using ArenaDefender.Game.Core;
using ArenaDefender.Game.Managers;
using ArenaDefender.Game.Entities;
using System.Collections.Generic;
using ArenaDefender.Game.Effects;

namespace ArenaDefender.Game;

/// <summary>
/// Top-level MonoGame entry point. Owns every manager/system and wires
/// input, updates and rendering together for each <see cref="GameState"/>.
/// Game *logic* (health, scoring, spawning, math) intentionally lives in the
/// Core/Entities/Managers classes so it can be unit tested independently of
/// this class, which is concerned only with the MonoGame loop and drawing.
/// </summary>
public class Game1 : Microsoft.Xna.Framework.Game
{
    private readonly GameManager _gameManager;
    private readonly InputManager _inputManager;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private EnemyManager _enemyManager;
    private PowerUpManager _powerUpManager;
    private Texture2D _pixel;
    private SpriteFont _font;
    private ProjectileManager _projectileManager;
    private ExperienceManager _experienceManager;
    private float _shootTimer;
    private float _shootInterval = 0.5f;
    private float _pulseTime = 0f;
    private ParticleManager _particleManager;
    private CameraShake _cameraShake;

    private RippleManager _rippleManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _gameManager = new GameManager();
        _inputManager = new InputManager();
        _enemyManager = new EnemyManager();
        _powerUpManager = new PowerUpManager();
        _projectileManager = new ProjectileManager();
        _experienceManager = new ExperienceManager();
        _particleManager = new ParticleManager();
        _cameraShake = new CameraShake();
        _rippleManager = new RippleManager();
    }

    protected override void Initialize()
    {
        try
        {
            _player = new Player(new Vector2(400, 300));
            _enemyManager.AddEnemy(new StandardEnemy(new Vector2(100, 100)));
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Initialize failed: {ex.Message}");
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        try
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            _font = Content.Load<SpriteFont>("DefaultFont");
        }
        catch (ContentLoadException ex)
        {
            // Content is missing or corrupt - fail loudly in the debug
            // output rather than crashing with an unhandled exception, so
            // the rest of the game can still attempt to run.
            System.Diagnostics.Debug.WriteLine($"Failed to load game content: {ex.Message}");
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _inputManager.Update();

        try
        {
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

                case GameState.LevelUp:
                    UpdateLevelUp(gameTime);
                    break;
            }
        }
        catch (System.Exception ex)
        {
            // A single bad frame shouldn't crash the whole game - log it and
            // keep running so the player doesn't lose their session.
            System.Diagnostics.Debug.WriteLine($"Update error in state {_gameManager.State}: {ex.Message}");
        }

        base.Update(gameTime);
    }

    private void UpdateMenu(GameTime gameTime)
    {
        _pulseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_inputManager.IsKeyPressed(Keys.Enter))
        {
            _gameManager.StartGame();
        }
    }

    private void UpdateGameplay(GameTime gameTime)
    {
        _pulseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _player.Update(gameTime, _inputManager);
        _enemyManager.Update(gameTime, _player.Position);
        _projectileManager.Update(gameTime);
        _powerUpManager.Update(gameTime);
        _particleManager.Update(gameTime);
        _cameraShake.Update(gameTime);
        _rippleManager.Update(gameTime);
        HandleCollisions();
        if (_experienceManager.LevelUpReady)
        {
            _gameManager.LevelUp();
        }
        if (_player.IsDead)
        {
            _gameManager.GameOver();
        }
        _shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_shootTimer >= _shootInterval)
        {
            _shootTimer = 0f;
            ShootNearestEnemy();
        }
    }

    private void UpdateGameOver(GameTime gameTime)
    {
        if (_inputManager.IsKeyPressed(Keys.Enter))
        {
            _gameManager.Reset();
            _gameManager.StartGame();

            _player = new Player(new Vector2(400, 300));
            _enemyManager = new EnemyManager();
            _powerUpManager = new PowerUpManager();
            _projectileManager = new ProjectileManager();
            _experienceManager = new ExperienceManager();
        }
    }

    private void UpdateLevelUp(GameTime gameTime)
    {
        if (_inputManager.IsKeyPressed(Keys.D1))
        {
            _player.IncreaseSpeed(50f);

            _experienceManager.FinishLevelUp();
            _gameManager.StartGame();
        }

        else if (_inputManager.IsKeyPressed(Keys.D2))
        {
            _shootInterval *= 0.8f;

            _experienceManager.FinishLevelUp();
            _gameManager.StartGame();
        }

        else if (_inputManager.IsKeyPressed(Keys.D3))
        {
            _player.IncreaseMaxHealth(25);

            _experienceManager.FinishLevelUp();
            _gameManager.StartGame();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        float r = 18 + 10 * (float)System.Math.Sin(_pulseTime * 0.20f);
        float g = 20 + 8 * (float)System.Math.Sin(_pulseTime * 0.33f);
        float b = 40 + 15 * (float)System.Math.Sin(_pulseTime * 0.15f);

        GraphicsDevice.Clear(new Color((int)r, (int)g, (int)b));

        _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(_cameraShake.Offset.X, _cameraShake.Offset.Y, 0));

        if (_gameManager.State == GameState.Menu)
        {
            DrawMenu();
        }
        else if (_gameManager.State == GameState.Playing)
        {
            DrawGameplay();
        }
        else if (_gameManager.State == GameState.LevelUp)
        {
            _spriteBatch.DrawString(
                _font,
                "LEVEL UP!",
                new Vector2(250, 120),
                Color.Gold);

            _spriteBatch.DrawString(
                _font,
                "Press 1 - Increase Speed",
                new Vector2(180, 180),
                Color.White);

            _spriteBatch.DrawString(
                _font,
                "Press 2 - Faster Shooting",
                new Vector2(180, 220),
                Color.White);

            _spriteBatch.DrawString(
                _font,
                "Press 3 - Increase Max Health",
                new Vector2(180, 260),
                Color.Gray);
        }
        else if (_gameManager.State == GameState.GameOver)
        {
            DrawGameOver();
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawMenu()
    {
        // Fade the title in over the first second on screen using a lerp
        // rather than snapping it straight to full opacity.
        float titleAlpha = MathHelpers.Lerp(0f, 1f, System.Math.Min(_pulseTime, 1f));

        float pulse = 1f + 0.05f * (float)System.Math.Sin(_pulseTime * 2f);

        _spriteBatch.DrawString(
            _font,
            "ARENA DEFENDER",
            new Vector2(230, 180),
            Color.DeepSkyBlue * titleAlpha,
            0f,
            Vector2.Zero,
            1.8f * pulse,
            SpriteEffects.None,
            0f);


        _spriteBatch.DrawString(
            _font,
            "Press ENTER to Start Game",
            new Vector2(260, 340),
            Color.White * titleAlpha);

        _spriteBatch.DrawString(
            _font,
            "WASD/Arrows to move and ESC to quit",
            new Vector2(80, 420),
            Color.Gray * titleAlpha);
    }

    private void DrawGameOver()
    {
        _spriteBatch.DrawString(
            _font,
            "GAME OVER",
            new Vector2(260, 180),
            Color.OrangeRed);

        _spriteBatch.DrawString(
            _font,
            $"Final Score: {_gameManager.Score}",
            new Vector2(260, 240),
            Color.White);

        _spriteBatch.DrawString(
            _font,
            $"Wave Reached: {_enemyManager.Wave}",
            new Vector2(260, 270),
            Color.White);

        _spriteBatch.DrawString(
            _font,
            "Press ENTER to Play Again",
            new Vector2(230, 340),
            Color.LightGray);
    }

    private void DrawGameplay()
    {
        float pulse = 40 + (float)System.Math.Sin(_pulseTime * 5f) * 3f;
        _spriteBatch.Draw(
            _pixel,
            new Rectangle(
                (int)_player.Position.X,
                (int)_player.Position.Y,
                (int)pulse,
                (int)pulse),
            _player.ShieldActive ? Color.Cyan : Color.DeepSkyBlue);

        _spriteBatch.Draw(
            _pixel,
            new Rectangle(10, 10, 200, 20),
            Color.DarkGray);

        _spriteBatch.DrawString(
            _font,
            $"Score: {_gameManager.Score}",
            new Vector2(10, 40),
            Color.White);

        _spriteBatch.DrawString(
            _font,
            $"Wave: {_enemyManager.Wave}",
            new Vector2(10, 70),
            Color.White);

        _spriteBatch.DrawString(
                _font,
                $"Health: {_player.Health}/{_player.MaxHealth}",
                new Vector2(10, 100),
                Color.White);

        _spriteBatch.DrawString(
            _font,
            $"Level: {_experienceManager.Level}",
            new Vector2(10, 130),
            Color.White);

        _spriteBatch.DrawString(
            _font,
            $"XP: {_experienceManager.Experience}/{_experienceManager.ExperienceToNextLevel}",
            new Vector2(10, 160),
            Color.White);

        // health bar damage/healing animates smoothly instead of snapping instantly using lerp
        float healthFraction = _player.DisplayedHealth / _player.MaxHealth;
        _spriteBatch.Draw(
            _pixel,
            new Rectangle(10, 10, (int)(200 * healthFraction), 20),
            Color.LimeGreen);

        foreach (Ripple ripple in _rippleManager.Ripples)
        {
            int segments = 40;

            for (int i = 0; i < segments; i++)
            {
                float angle = MathHelper.TwoPi * i / segments;

                Vector2 point =
                    ripple.Position +
                    new Vector2(
                        (float)System.Math.Cos(angle),
                        (float)System.Math.Sin(angle))
                    * ripple.Radius;

                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle(
                        (int)point.X,
                        (int)point.Y,
                        3,
                        3),
                    Color.Cyan * (1f - ripple.Radius / ripple.MaxRadius));
            }
        }

        foreach (PowerUp powerUp in _powerUpManager.PowerUps)
        {
            Color color = powerUp.Type switch
            {
                PowerUpType.Health => Color.LimeGreen,
                PowerUpType.SpeedBoost => Color.Yellow,
                PowerUpType.Shield => Color.Cyan,
                _ => Color.Magenta
            };

            float pOpacity = MathHelpers.Clamp(powerUp.TimeToLive / 3f, 0.3f, 1f);

            _spriteBatch.Draw(
                _pixel,
                new Rectangle(
                    (int)powerUp.Position.X,
                    (int)powerUp.Position.Y,
                    26,
                    26),
                color * pOpacity);
        }

        foreach (Enemy enemy in _enemyManager.Enemies)
        {
            Color color = enemy switch
            {
                FastEnemy => Color.LimeGreen,
                TankEnemy => Color.DarkRed,
                _ => Color.Red
            };
            if (enemy.IsFlashing)
            {
                color = Color.White;
            }

            float enemyPulse = 40 +
                (float)System.Math.Sin(
                    _pulseTime * 6f +
                    enemy.Position.X * 0.02f +
                    enemy.Position.Y * 0.02f) * 3f;

            _spriteBatch.Draw(
                _pixel,
                new Rectangle(
                    (int)(enemy.Position.X - (enemyPulse - 40) / 2),
                    (int)(enemy.Position.Y - (enemyPulse - 40) / 2),
                    (int)enemyPulse,
                    (int)enemyPulse),
                color);
        }
        foreach (Projectile projectile in _projectileManager.Projectiles)
        {
            int i = 0;

            foreach (Vector2 trail in projectile.Trail)
            {
                float alpha = (float)i / projectile.Trail.Count;

                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle(
                        (int)trail.X,
                        (int)trail.Y,
                        6,
                        6),
                    Color.Yellow * alpha);

                i++;
            }
            _spriteBatch.Draw(
                _pixel,
                new Rectangle(
                    (int)projectile.Position.X,
                    (int)projectile.Position.Y,
                    10,
                    10),
                Color.Yellow);
        }
        foreach (Particle particle in _particleManager.Particles)
        {
            _spriteBatch.Draw(
                _pixel,
                new Rectangle(
                    (int)particle.Position.X,
                    (int)particle.Position.Y,
                    (int)particle.Size,
                    (int)particle.Size),
                particle.Color * particle.Life);
        }
    }

    private void ShootNearestEnemy()
    {
        Enemy? nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Enemy enemy in _enemyManager.Enemies)
        {
            float distance = MathHelpers.Distance(_player.Position, enemy.Position);

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
                _player.Position + new Vector2(20, 20),
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
                    _rippleManager.AddRipple(
                        projectile.Position,
                        60f,
                        500f);

                    usedProjectiles.Add(projectile);

                    if (enemy.IsDead)
                    {
                        deadEnemies.Add(enemy);
                        _particleManager.CreateExplosion(
                        enemy.Position + new Vector2(20, 20),
                        Color.OrangeRed);
                        _rippleManager.AddRipple(
                            enemy.Position + new Vector2(20, 20),
                            180f,
                            300f);
                        _cameraShake.Shake(1.5f, 0.03f);
                        _cameraShake.Shake(4f, 0.08f);
                        _gameManager.AddScore(enemy.ScoreValue);
                        _experienceManager.AddExperience(enemy.ScoreValue);
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

        List<Enemy> playerHitEnemies = new();

        foreach (Enemy enemy in _enemyManager.Enemies)
        {
            if (enemy.Bounds.Intersects(_player.Bounds))
            {
                // Passing the enemy's position lets the player check
                // (via dot product) whether it was struck from behind.
                _player.TakeDamage(enemy.Damage, enemy.Position);
                playerHitEnemies.Add(enemy);
            }
        }

        foreach (Enemy enemy in playerHitEnemies)
        {
            _enemyManager.RemoveEnemy(enemy);
        }

        // Player <-> power-up collectible collision.
        List<PowerUp> collectedPowerUps = new();

        foreach (PowerUp powerUp in _powerUpManager.PowerUps)
        {
            if (powerUp.Bounds.Intersects(_player.Bounds))
            {
                ApplyPowerUp(powerUp);
                collectedPowerUps.Add(powerUp);
            }
        }

        foreach (PowerUp powerUp in collectedPowerUps)
        {
            _powerUpManager.Remove(powerUp);
            _gameManager.AddScore(5);
        }
    }

    private void ApplyPowerUp(PowerUp powerUp)
    {
        switch (powerUp.Type)
        {
            case PowerUpType.Health:
                _player.IncreaseHealth(30);
                break;

            case PowerUpType.SpeedBoost:
                _player.IncreaseSpeed(40f);
                break;

            case PowerUpType.Shield:
                _player.ActivateShield(6f);
                break;

            case PowerUpType.DamageBoost:
                _shootInterval = System.Math.Max(0.1f, _shootInterval * 0.85f);
                break;
        }

        _particleManager.CreateExplosion(powerUp.Position, Color.Gold);
    }
}
