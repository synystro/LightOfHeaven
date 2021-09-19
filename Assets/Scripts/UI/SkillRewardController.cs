using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;

namespace LUX.LightOfHeaven {
    public class SkillRewardController : MonoBehaviour {

        public List<SkillReward> skillRewards;
        private int slots;

        [Inject] LootGenerator lootGenerator;        

        private void Awake() {
            skillRewards = this.GetComponentsInChildren<SkillReward>(true).ToList();
            slots = skillRewards.Count;
        }
        private void OnEnable() {
            if (skillRewards.Any() == false) { Debug.Log("Skill reward buttons not obtained!"); return; }

            Spell[] skills = new Spell[slots];

            for(int i = 0; i < slots; i++) {
                skills[i] = lootGenerator.GetRandomSkill();
            }

            int j = 0;
            skillRewards.ForEach(s => {
                s.SetSkill(skills[j]);
                j++;
            });
        }
        private void OnDisable() {
            skillRewards.ForEach(s => s.SetSkill(null));
        }
    }
}
