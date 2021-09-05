#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LUX.LightOfHeaven {
    public class Item : ScriptableObject {
        [SerializeField] string id;
        public string Id { get { return id; } }
        public string Name;
        public Sprite Icon;
        [TextArea(1, 3)]
        public string Description;

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            string path = AssetDatabase.GetAssetPath(this);
        }
#endif

        public virtual Item GetCopy() {
            return this;
        }

        public virtual void Destroy() {

        }

        public virtual string GetItemType() {
            return "";
        }

        public virtual string GetDescription() {
            return "";
        }
    }
}
