
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using static ScatteredStones.ResourceBank.ThingDefOf;

namespace ScatteredStones
{
	[HarmonyPatch(typeof(Mineable), nameof(Mineable.DestroyMined))]
	public class Patch_Destroy
	{
		//Unlike the filth under the rock chunks, the ones under mineables never get their timers reset. To prevent despawning immediately, we do the reset upon mining.
		static public void Prefix(ref Mineable __instance)
		{	
			var rocks = __instance.Map.thingGrid.ThingsListAtFast(__instance.Position).FirstOrDefault(x => x.def == Owl_Filth_Rocks) as Filth;
			if (rocks != null) rocks.ThickenFilth();
		}
    }

	[HarmonyPatch(typeof(Building), nameof(Building.SpawnSetup))]
	public class Patch_SpawnSetup
	{
		//When a new building is spawned, we check the adjacent cells to see if it created an impassible situtation that makes filth unreachable, which could otherwise cause the standing bug
		static public void Postfix(ref Building __instance)
		{
			if (__instance == null) return;

			Map map = __instance.Map;
			if (map.AgeInDays == 0) return;

			if (__instance.Position != null)
			{
				__instance.OccupiedRect().AdjacentCellsCardinal.Where(x => x.InBounds(map) && x.Impassable(map)).ToList().ForEach(y => map.GetComponent<MapComponent_ScatteredStones>().ValidateCell(y, true));
			}
		}
    }
}
