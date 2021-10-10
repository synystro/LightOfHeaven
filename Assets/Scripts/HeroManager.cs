using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    public class HeroManager : MonoBehaviour {
        public Unit Hero => hero;
        [SerializeField] private Unit hero;

        [Inject] GameEventSystem gameEventSystem;
        [Inject] UnitManager unitManager;

        private void OnEnable() {
            gameEventSystem.onHeroChosen += OnHeroChosen;
        }
        private void OnDisable() {
            gameEventSystem.onHeroChosen -= OnHeroChosen;            
        }
        private void OnHeroChosen(Unit hero) {
            this.hero = hero;
        }
        public void AddSkill(Spell skill) {
            hero.Spells.Add(skill);
            gameEventSystem.OnSkillAdded(skill);
        }
        public void AddEquip(Equippable equip) {
            bool success = hero.Equip(equip, unitManager.Player);
            if(success)
                gameEventSystem.OnEquipped(equip);
        }
    }
}