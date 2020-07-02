using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Industropolis.Engine
{
    public struct VKeyboardState<TEnum> where TEnum : notnull
    {
        private Dictionary<TEnum, VKey> _keys;

        public VKeyboardState(Dictionary<TEnum, VKey> keys) => _keys = keys;

        public bool IsPressed(TEnum key)
        {
            _keys.TryGetValue(key, out var vKey);
            return vKey != null && vKey.Pressed;
        }

        public bool IsHeld(TEnum key)
        {
            _keys.TryGetValue(key, out var vKey);
            return vKey != null && vKey.Held;
        }

        public float GetHeldTime(TEnum key)
        {
            _keys.TryGetValue(key, out var vKey);
            return vKey == null ? 0 : vKey.HeldTime;
        }
    }

    public static class VKeyboard<TEnum> where TEnum : notnull
    {
        private static Dictionary<TEnum, VKey> keys = new Dictionary<TEnum, VKey>();

        public static VKey AddKey(TEnum key, Keys[] trigger)
        {
            var vKey = new VKey();
            vKey.AddTrigger(trigger);
            keys.Add(key, vKey);

            return vKey;
        }

        public static VKeyboardState<TEnum> GetState() => new VKeyboardState<TEnum>(keys);

        public static void AddKeys(IEnumerable<(TEnum key, Keys[] trigger)> keys)
        {
            foreach (var key in keys) AddKey(key.key, key.trigger);
        }

        public static void AddKeys(IEnumerable<(TEnum key, Keys trigger)> keys)
        {
            foreach (var key in keys) AddKey(key.key, new[] { key.trigger });
        }

        public static VKey GetKey(TEnum key)
        {
            return keys[key];
        }

        public static void Update(KeyboardState kstate, float elapsedTime)
        {
            foreach (var key in keys.Values)
            {
                key.SetKeyboardState(kstate, elapsedTime);
            }
        }
    }

    public class VKey
    {
        List<Keys[]> triggers = new List<Keys[]>();
        KeyboardState prevstate, kstate;
        float elapsed;
        private float heldtime;
        public float HeldTime
        {
            get
            {
                if (Held) heldtime += elapsed;
                else heldtime = 0;

                return heldtime;
            }
        }

        public bool Held
        {
            get
            {
                return (KeysDown(kstate) && KeysDown(prevstate));
            }
        }

        public bool Pressed
        {
            get
            {
                return (KeysDown(kstate) && !KeysDown(prevstate));
            }
        }

        public void AddTrigger(Keys[] keys)
        {
            if (keys != null) triggers.Add(keys);
        }

        public void SetKeyboardState(KeyboardState currentState, float elapsedTime)
        {
            prevstate = kstate;
            kstate = currentState;
            elapsed = elapsedTime;
        }

        private bool KeysDown(KeyboardState kstate)
        {
            foreach (Keys[] keys in triggers)
            {
                foreach (Keys key in keys)
                {
                    if (kstate.IsKeyUp(key)) return false;
                }
            }

            return true;
        }
    }
}