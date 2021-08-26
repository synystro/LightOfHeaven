using System;
using UnityEngine;

namespace LUX.LightOfHeaven {
    public class GameEventSystem : MonoBehaviour {
        public event Action onPlayerSpawned;
        public void OnPlayerSpawned() { onPlayerSpawned?.Invoke(); }
        public event Action onBattleStarted;
        public void OnBattleStarted() { onBattleStarted?.Invoke(); print("battle started!"); }
        public event Action onBattleEnded;
        public void OnBattleEnded() { onBattleEnded?.Invoke(); print("battle ended!"); }
        public event Action onTurnStarted;
        public void OnTurnStarted() { onTurnStarted?.Invoke(); }
        public event Action onTurnEnded;
        public void OnTurnEnded() { onTurnEnded?.Invoke(); }
        public event Action<bool> onUnitMoved;
        public void OnUnitMoved(bool isEnemy) { onUnitMoved?.Invoke(isEnemy); }
        public event Action<bool> onUnitAttacked;
        public void OnUnitAttacked(bool isEnemy) { onUnitAttacked?.Invoke(isEnemy); }
        public event Action<GameObject> onUnitDied;
        public void OnUnitDied(GameObject unitToDie) { onUnitDied?.Invoke(unitToDie); }
        public event Action onNextRoomLoaded;
        public void OnNextRoomLoaded() { onNextRoomLoaded?.Invoke(); }
        public event Action onPeacefulRoomLoaded;
        public void OnPeacefulRoomLoaded() { onPeacefulRoomLoaded?.Invoke(); print("entered peaceful room"); }
    }
}