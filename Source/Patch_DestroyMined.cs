
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using static ScatteredStones.ResourceBank.ThingDefOf;

namespace ScatteredStones
{
	[HarmonyPatch(typeof(Mineable), "DestroyMined")]
	public class Patch_Destroy
	{
		//Unlike the filth under the rock chunks, the ones under mineables never get their timers reset. To prevent despawning immediately, we do the reset upon mining.
		static public void Prefix(ref Mineable __instance)
		{	
			var rocks = __instance.Map.thingGrid.ThingsListAtFast(__instance.Position).FirstOrDefault(x => x.def == Owl_Filth_Rocks) as Filth;
			if (rocks != null) rocks.ThickenFilth();
		}
    }

	[HarmonyPatch(typeof(Building), "SpawnSetup")]
	public class Patch_SpawnSetup
	{
		//Unlike the filth under the rock chunks, the ones under mineables never get their timers reset. To prevent despawning immediately, we do the reset upon mining.
		static public void Postfix(ref Building __instance)
		{
			if (__instance == null) return;

			Map map = __instance.Map;
			if (map.AgeInDays == 0) return;
			if (__instance.Position != null && __instance.def.passability == Traversability.Impassable)
			{
				GenAdjFast.AdjacentCellsCardinal(__instance.Position).Where(x => x.InBounds(map)).ToList().ForEach(y => map.GetComponent<MapComponent_ScatteredStones>()?.ValidateCell(y, true));
			}
		}
    }
}
