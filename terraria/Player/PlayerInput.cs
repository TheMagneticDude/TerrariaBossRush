using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;

namespace terraria.Players;

public enum KeyAction 
{ 
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Attack,
    SelectSlot1,
    SelectSlot2,
    SelectSlot3,
    SelectSlot4,
    SelectSlot5,
    SelectSlot6,
    SelectSlot7,
    SelectSlot8,
    SelectSlot9,
    SelectSlot10,
    UseItem,
    TakeDamage,
    Exit,
    Reset,
    PreviousRoom,
    NextRoom,
    TriggerDebug,
    ShowHitboxes,
    VoidFloor
}


public class PlayerInput : IPlayerInput
{
    public Dictionary<KeyAction, MultiBind> KeyBinds { get; private set; }
    InputManager Inputs {get;set;}

    public float DAS_delay = 0.3f;

    public PlayerInput(InputManager Input)
    {
        Inputs = Input;
        KeyBinds = new Dictionary<KeyAction, MultiBind>
        {
            { KeyAction.MoveUp,    new MultiBind(new TriggerKey(Keys.W), new TriggerKey(Keys.Up), new TriggerKey(Keys.Space)) },
            { KeyAction.MoveDown,  new MultiBind(new TriggerKey(Keys.S), new TriggerKey(Keys.Down)) },
            { KeyAction.MoveLeft,  new MultiBind(new TriggerKey(Keys.A), new TriggerKey(Keys.Left)) },
            { KeyAction.MoveRight, new MultiBind(new TriggerKey(Keys.D), new TriggerKey(Keys.Right)) },
            { KeyAction.Attack,    new MultiBind(new TriggerKey(Keys.Z), new TriggerKey(Keys.N), new TriggerKey(MouseButton.Left,useDAS: true, dasTimeMs: 0.5)) },
            { KeyAction.SelectSlot1,  new MultiBind(new TriggerKey(Keys.D1), new TriggerKey(Keys.NumPad1)) },
            { KeyAction.SelectSlot2,  new MultiBind(new TriggerKey(Keys.D2), new TriggerKey(Keys.NumPad2)) },
            { KeyAction.SelectSlot3,  new MultiBind(new TriggerKey(Keys.D3), new TriggerKey(Keys.NumPad3)) },
            { KeyAction.SelectSlot4, new MultiBind(new TriggerKey(Keys.D4), new TriggerKey(Keys.NumPad4)) },
            { KeyAction.SelectSlot5, new MultiBind(new TriggerKey(Keys.D5), new TriggerKey(Keys.NumPad5)) },
            { KeyAction.SelectSlot6, new MultiBind(new TriggerKey(Keys.D6), new TriggerKey(Keys.NumPad6)) },
            { KeyAction.SelectSlot7, new MultiBind(new TriggerKey(Keys.D7), new TriggerKey(Keys.NumPad7)) },
            { KeyAction.SelectSlot8, new MultiBind(new TriggerKey(Keys.D8), new TriggerKey(Keys.NumPad8)) },
            { KeyAction.SelectSlot9, new MultiBind(new TriggerKey(Keys.D9), new TriggerKey(Keys.NumPad9)) },
            { KeyAction.SelectSlot10, new MultiBind(new TriggerKey(Keys.D0), new TriggerKey(Keys.NumPad0)) },
            { KeyAction.UseItem, new MultiBind(new TriggerKey(MouseButton.Right)) },
            { KeyAction.TakeDamage, new MultiBind(new TriggerKey(Keys.E)) },
            { KeyAction.Exit,      new MultiBind(new TriggerKey(Keys.Q), new TriggerKey(Keys.Escape)) },
            { KeyAction.Reset,     new MultiBind(new TriggerKey(Keys.R)) },
            { KeyAction.PreviousRoom, new MultiBind(new TriggerKey(Keys.J)) },
            { KeyAction.NextRoom, new MultiBind(new TriggerKey(Keys.K)) },
            { KeyAction.TriggerDebug, new MultiBind(new TriggerKey(Keys.F3))},
            { KeyAction.ShowHitboxes, new MultiBind(new TriggerKey(Keys.B))},
            { KeyAction.VoidFloor, new MultiBind(new TriggerKey(Keys.G))}
        };
    }

    public bool IsPressed(KeyAction action)
    {
        return KeyBinds.TryGetValue(action, out var group) && group.IsPressed;
    }

    public bool IsNewPress(KeyAction action)
    {
        return KeyBinds.TryGetValue(action, out var group) && group.IsNewPress;
    }

    public void AddKey(KeyAction action, Keys keyCode)
    {
        if (KeyBinds.TryGetValue(action, out var group))
        {
            group.AddKey(new TriggerKey(keyCode));
        }
        else
        {
            KeyBinds[action] = new MultiBind(new TriggerKey(keyCode));
        }
    }

    public Point GetCursorPos()
    {
        return Inputs.Mouse.Position;
    }

    

    public void Update(GameTime gameTime)
    {
        foreach (var group in KeyBinds.Values)
        {
            group.Update(gameTime, Inputs, DAS_delay);
        }
    }

    public Keys[] ScanKey(KeyboardInfo keyboard)
    {
        ArgumentNullException.ThrowIfNull(keyboard);
        return keyboard.CurrentState.GetPressedKeys();
    }
}
