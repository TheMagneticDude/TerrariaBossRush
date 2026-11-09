using System.Collections.Generic;
using Microsoft.Xna.Framework;

using MonoGameLibrary.Input;

namespace terraria.Players;

public class MultiBind
{
    private List<TriggerKey> _triggers;

    

    // returns true if ANY key in this group is currently held down
    public bool IsPressed
    {
        get
        {
            for (int i = 0; i < _triggers.Count; i++)
            {
                if (_triggers[i].IsPressed) return true;
            }
            return false;
        }
    }

    // rturns true if ANY key in this group was just pressed this frame
    public bool IsNewPress
    {
        get
        {
            for (int i = 0; i < _triggers.Count; i++)
            {
                if (_triggers[i].IsNewPress) return true;
            }
            return false;
        }
    }

    //allows any number ofkeys
    public MultiBind(params TriggerKey[] keys)
    {
        _triggers = new List<TriggerKey>(keys);
    }

    public void AddKey(TriggerKey key)
    {
        _triggers.Add(key);
    }

    public void ReplaceKey(TriggerKey newKey, int slot = 0)
    {
        if (slot >= 0 && slot < _triggers.Count)
        {
            _triggers[slot] = newKey;
        }
        else
        {
            _triggers.Add(newKey);
        }
    }

    public void Update(GameTime gameTime, InputManager inputs, float dasDelay)
    {
        
        for (int i = 0; i < _triggers.Count; i++)
        {
            var key = _triggers[i];
            key.Update(gameTime, inputs);

            // Handle DAS logic per key
            if (key.UseDAS && key.LastDasTime >= key.DasTimeMs)
            {
                key.LastDasTime = 0;
                if (key.HoldTime > dasDelay) 
                {
                    key.ResetHold();
                }
            }
        }
    }
}