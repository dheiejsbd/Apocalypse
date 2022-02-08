using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse.Player
{
    public enum KeyID
    {
        Jump,
        Run,
        Crouch,
        Forward,
        Backward,
        Rightward,
        Leftward,

    }
    public class KeyCodeMap
    {
        public static KeyCodeMap instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KeyCodeMap();
                    instance.Initialize();
                }
                return instance;
            }
            private set { instance = value; }
        }


        private Dictionary<KeyID, KeyCode> Code = new Dictionary<KeyID, KeyCode>();

        public void Initialize()
        {
            TryAdd(KeyID.Jump, KeyCode.Space);
            TryAdd(KeyID.Run, KeyCode.LeftShift);
            TryAdd(KeyID.Crouch, KeyCode.LeftControl);
            TryAdd(KeyID.Forward, KeyCode.W);
            TryAdd(KeyID.Backward, KeyCode.S);
            TryAdd(KeyID.Rightward, KeyCode.D);
            TryAdd(KeyID.Leftward, KeyCode.A);
        }

        public void TryAdd(KeyID ID, KeyCode value)
        {
            if (Code.ContainsKey(ID)) return;
            Code.Add(ID, value);
        }

        public KeyCode TryGetKeyID(KeyID ID)
        {
            if (!Code.ContainsKey(ID)) return KeyCode.None;
            return Code[ID];

        }
    }
}
