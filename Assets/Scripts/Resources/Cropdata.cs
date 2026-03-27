using System.Collections.Generic;
using UnityEngine;

namespace WhereFirefliesReturn.Resources
{
    public enum CropType
    {
        None,
        SweetPotato,
        Corn,
        WaterSpinach,
        Marigold
    }

    /// <summary>
    /// Defines which crops are companions (help each other) and which compete.
    /// Based on the puzzle lore: sweet potato+corn, water spinach on edges, marigolds at border.
    /// </summary>
    public static class CompanionData
    {
        // Companion pairs — planting these next to each other is CORRECT
        private static readonly HashSet<(CropType, CropType)> companions = new()
        {
            (CropType.SweetPotato, CropType.Corn),
            (CropType.Corn, CropType.SweetPotato),
            (CropType.WaterSpinach, CropType.WaterSpinach), // edges next to each other is fine
            (CropType.Marigold, CropType.SweetPotato),      // marigold protects nearby crops
            (CropType.SweetPotato, CropType.Marigold),
            (CropType.Marigold, CropType.Corn),
            (CropType.Corn, CropType.Marigold),
        };

        // Competing pairs — these harm each other
        private static readonly HashSet<(CropType, CropType)> competitors = new()
        {
            (CropType.Corn, CropType.Corn),         // too many corn competes for light
            (CropType.WaterSpinach, CropType.Corn), // water spinach hogs moisture from corn
            (CropType.Corn, CropType.WaterSpinach),
        };

        public static bool AreCompanions(CropType a, CropType b)
        {
            if (a == CropType.None || b == CropType.None) return false;
            return companions.Contains((a, b));
        }

        public static bool AreCompetitors(CropType a, CropType b)
        {
            if (a == CropType.None || b == CropType.None) return false;
            return competitors.Contains((a, b));
        }

        public static string GetCompanionHint(CropType crop)
        {
            return crop switch
            {
                CropType.SweetPotato => "Sweet potato breaks up hard soil — corn grows tall beside it.",
                CropType.Corn        => "Corn reaches for the sky. It needs a root-friend below.",
                CropType.WaterSpinach => "Water spinach holds moisture. Plant it along the edges.",
                CropType.Marigold    => "Marigolds guard the border. No chemicals needed.",
                CropType.None        => "An empty bed, waiting."
            };
        }
    }
}