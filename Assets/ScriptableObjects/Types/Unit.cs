using UnityEngine;
using System.Collections.Generic;

namespace LUX.LightOfHeaven {
    [System.Serializable]
    [CreateAssetMenu(menuName = "LOH/Unit", fileName = "New Unit")]
    public class Unit : ScriptableObject {
        public new string name;
        public Sprite icon;
        public GameObject charPrefabRight;
        public GameObject charPrefabLeft;
        [Header("SPELLS")]
        public List<Spell> Spells;
        //[Header("EFFECTS")]
        //[SerializeField] public List<EffectData> ActiveEffects;
        [Header("FEATURES")]
        public bool Flight;
        [Header("ATTRIBUTES")]
        public int Strength;
        public int Stamina;
        public int Vitality;
        public int Dexterity;
        public int Intelligence;
        [Header("ESSENCE")]
        public int Hp;
        public int Ep;
        public int Sp;
        [Header("OFFENSE")]
        public int PhyDamage;
        public int MagDamage;
        public int AtkRange;
        public int AtkAccuracy;
        [Header("DEFENSE")]
        public int PhyShield;
        public int MagShield;
        public int PhyArmor;
        public int MagArmor;
        public int Poise;

        private void Awake() {
            //ActiveEffects = new List<EffectData>();
        }
        public void AddSpell(Spell s) {
            Spells.Add(s);
        }
        public void RemoveSpell(Spell s) {
            if(Spells.Contains(s)) {
                Spells.Remove(s);
            }
        }
    }
}
