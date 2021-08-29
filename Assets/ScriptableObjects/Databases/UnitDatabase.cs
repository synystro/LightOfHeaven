using UnityEngine;

namespace LUX.LightOfHeaven {
    [CreateAssetMenu(menuName = "LOH/UnitDatabase", fileName = "New UnitDatabase")]
    public class UnitDatabase : ScriptableObject {
        [Header("HELL")]
        [Expandable]
        public Unit[] hellUnits;
        [Header("SKY")]
        [Expandable]
        public Unit[] skyUnits;
        [Header("SPACE")]
        [Expandable]
        public Unit[] spaceUnits;
    }
}
