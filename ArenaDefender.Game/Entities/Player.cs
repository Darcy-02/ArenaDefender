using Microsoft.Xna.Framework;
using ArenaDefender.Game.Managers;
using Microsoft.Xna.Framework.Input;

namespace ArenaDefender.Game.Entities;

public class Player
{
    public Vector2 Position { get; private set; }

    public float Speed { get; private set; }

    public int Health { get; private set; }

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        Speed = 250f;
        Health = 100;
    }

    public void Update(GameTime gameTime, InputManager input)
    {
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
        }

        Position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}
