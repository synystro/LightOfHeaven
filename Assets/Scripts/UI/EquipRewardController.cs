using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;

namespace LUX.LightOfHeaven {
    public class EquipRewardController : MonoBehaviour {

        public List<EquipReward> equipRewards;
        private int slots;

        [Inject] LootGenerator lootGenerator;        

        private void Awake() {
            equipRewards = this.GetComponentsInChildren<EquipReward>(true).ToList();
            slots = equipRewards.Count;
        }
        private void OnEnable() {
            if (equipRewards.Any() == false) { Debug.Log("Equipment reward buttons not obtained!"); return; }

            Equippable[] equipment = new Equippable[slots];

            for(int i = 0; i < slots; i++) {
                equipment[i] = lootGenerator.GetRandomEquipment();
            }

            int j = 0;
            equipRewards.ForEach(s => {
                s.SetEquip(equipment[j]);
                j++;
            });
        }
        private void OnDisable() {
            equipRewards.ForEach(s => s.SetEquip(null));
        }
    }
}
