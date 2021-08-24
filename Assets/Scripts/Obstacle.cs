using UnityEngine;

namespace LUX.LightOfHeaven {
    public class Obstacle : MonoBehaviour, IDestructible {
        [SerializeField] private int hp;
        public void Damage(int damage) {
            hp -= damage;
            DestructCheck();            
        }
        public void DestructCheck() {
            if(hp <= 0) {
                Destroy(this.gameObject);
            }                        
        }
    }
}