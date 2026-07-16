using Microsoft.Xna.Framework.Input;

namespace ArenaDefender.Game.Managers;

public class InputManager
{
    //compare previous frame to the current one to avoid every frame being started
    public KeyboardState KeyboardState { get; private set; }

    public KeyboardState PreviousKeyboardState { get; private set; }

    public void Update()
    {
        PreviousKeyboardState = KeyboardState;
        KeyboardState = Keyboard.GetState();
    }

    public bool IsKeyPressed(Keys key)
    {
        return KeyboardState.IsKeyDown(key) &&
               PreviousKeyboardState.IsKeyUp(key);
    }

    public bool IsKeyDown(Keys key)
    {
        return KeyboardState.IsKeyDown(key);
    }
}