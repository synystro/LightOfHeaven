using UnityEngine;
using Zenject;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace LUX.LightOfHeaven {
    public class RunInfoManager : MonoBehaviour {

        [Inject] GameEventSystem gameEventSystem;

        [SerializeField] private List<TextUi> texts;

        TextMeshProUGUI heroNameTMP;
        TextMeshProUGUI currentHpTMP;
        TextMeshProUGUI maxHpTMP;

        private void Awake() {
            texts = this.GetComponentsInChildren<TextUi>().ToList();
            heroNameTMP = texts.FirstOrDefault(t => t.Sid == "HeroName")?.GetComponent<TextMeshProUGUI>();
            currentHpTMP = texts.FirstOrDefault(t => t.Sid == "CurrentHp")?.GetComponent<TextMeshProUGUI>();
            maxHpTMP = texts.FirstOrDefault(t => t.Sid == "MaxHp")?.GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable() {
            gameEventSystem.onHeroChosen += OnHeroChosen;
            gameEventSystem.onPlayerHpChanged += OnPlayerHpChanged;
            gameEventSystem.onPlayerMaxHpChanged += OnPlayerMaxHpChanged;
        }

        private void OnDisable() {
            gameEventSystem.onPlayerHpChanged -= OnPlayerHpChanged;
            gameEventSystem.onPlayerMaxHpChanged -= OnPlayerMaxHpChanged;
        }

        private void OnHeroChosen(Unit unit) {
            if(heroNameTMP)
                heroNameTMP.text = unit.name.ToString();
            else
                Debug.LogError($"Text UI missing for this callback!");
        }

        private void OnPlayerHpChanged(int playerCurrentHp) {
            if(currentHpTMP)
                currentHpTMP.text = playerCurrentHp.ToString();
            else
                Debug.LogError($"Text UI missing for this callback!");
        }
        private void OnPlayerMaxHpChanged(int playerMaxHp) {
            if(maxHpTMP)
                maxHpTMP.text = playerMaxHp.ToString();
            else
                Debug.LogError($"Text UI missing for this callback!");
        }
    }
}