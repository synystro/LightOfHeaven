﻿using System;
using UnityEngine;

namespace LUX {
    public class GameEventSystem : MonoBehaviour {
        public event Action onPlayerSpawn;
        public void OnPlayerSpawn() { onPlayerSpawn?.Invoke(); }
        public event Action onTurnStart;
        public void OnTurnStart() { onTurnStart?.Invoke(); }
        public event Action onTurnEnd;
        public void OnTurnEnd() { onTurnEnd?.Invoke(); }
        public event Action<bool> onUnitMove;
        public void OnUnitMove(bool isEnemy) { onUnitMove?.Invoke(isEnemy); }
        public event Action<bool> onUnitAttack;
        public void OnUnitAttack(bool isEnemy) { onUnitAttack?.Invoke(isEnemy); }
        public event Action<GameObject> onUnitDie;
        public void OnUnitDie(GameObject unitToDie) { onUnitDie?.Invoke(unitToDie); }
    }
}