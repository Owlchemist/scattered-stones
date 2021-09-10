
using Verse;
using RimWorld;
using HarmonyLib;
using System.Linq;
using static ScatteredStones.ResourceBank.ThingCategoryDefOf;
 
namespace ScatteredStones
{
    [HarmonyPatch (typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PostResolve))]
    static class Patch_DefGenerator
    {
        static void Postfix()
        {
            Mod_ChunkyChunks.stoneChunks = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.thingCategories != null && x.thingCategories.Contains(StoneChunks));
            Mod_ChunkyChunks.stoneCliff = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.building != null && x.building.isNaturalRock && !x.building.isResourceRock);
        }
    }
}