using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    //TODO make this a scriptable obj system
    public class EquipmentSystem : MonoBehaviour {
        [Inject] GameEventSystem gameEventSystem;
        [Expandable][SerializeField] private Equippable weapon;

        public bool Equip(Equippable equipment, UnitController owner) {
            switch(equipment.Type) {
                case EquipmentType.Weapon: return AddWeapon(equipment, owner);
                default: Debug.LogError("Equipment type unkown?"); return false;
            }
        }
        private bool AddWeapon(Equippable weapon, UnitController owner) {
            if(this.weapon)
                return false;
            this.weapon = weapon;  
            weapon.Equip(owner.UnitStats);
            return true;
        }
    }
}
