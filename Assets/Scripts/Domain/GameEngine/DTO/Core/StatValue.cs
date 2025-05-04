using System;

using Assets.Scripts.Domain.GameEngine.Domain.Enums;

namespace Assets.Scripts.Domain.GameEngine.Domain.Core
{
    public readonly struct StatValue
    {
        public StatType StatType { get; }
        public int BaseValue { get; }

        public double Raw { get; }
        public double DiffDouble => Raw - BaseValue;

        public int Rounded => (int)Math.Round(Raw);
        public int DiffInt => Rounded - BaseValue;

        public StatValue(StatType statType, double raw, int baseValue)
        {
            StatType = statType;
            Raw = raw;
            BaseValue = baseValue;
        }

        public string Display
        {
            get
            {
                if (StatType == StatType.Initiative)
                {
                    if (Math.Abs(DiffDouble) < 1e-6)
                    {
                        return Raw.ToString("0.##");
                    }

                    var sign = DiffDouble > 0 ? "+" : "-";
                    var diff = Math.Abs(DiffDouble);
                    return $"{Raw:0.##} ({sign}{diff:0.##})";
                }

                if (DiffInt == 0)
                {
                    return Rounded.ToString();
                }

                var signInt = DiffInt > 0 ? "+" : "-";
                var diffInt = Math.Abs(DiffInt);
                return $"{Rounded} ({signInt}{diffInt})";
            }
        }
    }
}
