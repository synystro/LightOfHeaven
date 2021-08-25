using UnityEngine;
using System.Collections.Generic;

namespace LUX.LightOfHeaven {

    [CreateAssetMenu]
    public class GameEvent : ScriptableObject {
        private readonly List<GameEventListener> listeners = new List<GameEventListener>();

        public void Raise() {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised();
        }
        public void RegisterListener(GameEventListener listener) {
            if(listeners.Contains(listener) == false)
                listeners.Add(listener);
        }
        public void UnregisterListener(GameEventListener listener) {
            if(listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }
}
