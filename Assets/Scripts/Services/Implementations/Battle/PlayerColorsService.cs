using Assets.Scripts.Services.Interfaces.Battle;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class PlayerColorsService : IPlayerColorsService
    {
        private readonly uint[] _palette =
        {
            0xFF0000, // красный
            0x0000FF, // синий
            0xFFFF00, // жёлтый
            0x00FF00, // зелёный
        };

        private readonly Dictionary<int, uint> _cache = new();

        public uint GetRgb24(int playerId)
        {
            if (_cache.TryGetValue(playerId, out var rgb))
            {
                return rgb;
            }

            rgb = _palette[_cache.Count % _palette.Length];
            _cache[playerId] = rgb;
            return rgb;
        }
    }
}
