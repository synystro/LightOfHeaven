namespace LUX.LightOfHeaven {
    public enum DamageType { None, Physical, Magical, Piercing }

    public struct DamageData {
        public UnitStats Source { get; private set; }
        public int Amount { get; private set; }
        public DamageType Type { get; private set; }
        public int CritChance { get; private set; }
        public int StunChance { get; private set; }
        public int LethalChance { get; private set; }

        public DamageData(UnitStats source, int amount, DamageType type, int critChance, int stunChance, int lethalChance) {
            this.Source = source;
            this.Amount = amount;
            this.Type = type;
            this.CritChance = critChance;
            this.StunChance = stunChance;
            this.LethalChance = lethalChance;
        }
    }
}