using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    public class HeroManager : MonoBehaviour {
        public Unit Hero => hero;
        [SerializeField] private Unit hero;

        [Inject] GameEventSystem gameEventSystem;

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
    }
}