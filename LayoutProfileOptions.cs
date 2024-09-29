using System.Collections.Generic;

namespace KitchenSpeedrunLayouts
{
    internal static class LayoutProfileOptions
    {
        public static Dictionary<int, string> SelectableLayoutProfiles => new Dictionary<int, string>
        {
            { 0, "Vanilla" },
            { -80202533, "Diner (Solo)" },
            { 222370461, "Basic (Duo, 10x7)" },
            { -2045800810, "Medium (Duo, 14x6)" },
            { 557943155, "Extended (Trio)" },
            { 154938708, "Huge (Quad)" },
            { -1207945624, "Lake (Non-Solo)" },

        };
    }
}
