using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    //TODO make this a scriptable obj system
    public class EquipmentSystem : MonoBehaviour {
        [Inject] GameEventSystem gameEventSystem;
        [Expandable][SerializeField] private Equippable weapon;

        public void Equip(Equippable equipment, UnitController owner) {
            switch(equipment.Type) {
                case EquipmentType.Weapon: AddWeapon(equipment, owner); break;
                default: Debug.LogError("Equipment type unkown?"); break;
            }
        }
        private void AddWeapon(Equippable weapon, UnitController owner) {
            if(this.weapon)
                return;     
            this.weapon = weapon;  
            weapon.Equip(owner.UnitStats);          
            gameEventSystem.OnEquipped(weapon);
        }
    }
}
