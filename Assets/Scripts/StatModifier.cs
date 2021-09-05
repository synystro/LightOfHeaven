namespace LUX.LightOfHeaven {
    public enum StatModType {
        Flat = 10,
        Percent = 20
    }
    public class StatModifier {
        public readonly int Value;
        public readonly StatModType Type;
        public readonly int Order;
        public readonly object Source;

        public StatModifier(int value, StatModType type, int order, object source) {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }
        public StatModifier(int value, StatModType type) : this (value, type, (int)type, null) {}
        public StatModifier(int value, StatModType type, int order) : this (value, type, order, null) {}
        public StatModifier(int value, StatModType type, object source) : this (value, type, (int)type, source) {}
    }
}
