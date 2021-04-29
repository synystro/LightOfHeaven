using System;
using UnityEngine;

public class GameEventSystem : MonoBehaviour {
    public event Action onTurnStart;
    public void OnTurnStart() { onTurnStart?.Invoke(); }
    public event Action onTurnEnd;
    public void OnTurnEnd() { onTurnEnd?.Invoke(); }
    public event Action<bool> onUnitMove;
    public void OnUnitMove(bool isEnemy) { onUnitMove?.Invoke(isEnemy); }
}
