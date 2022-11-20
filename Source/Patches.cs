
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using static ScatteredStones.ResourceBank.ThingDefOf;
using static ScatteredStones.ModSettings_ScatteredStones;

namespace ScatteredStones
{
	//Unlike the filth under the rock chunks, the ones under mineables never get their timers reset. To prevent despawning immediately, we do the reset upon mining.
	[HarmonyPatch(typeof(Mineable), nameof(Mineable.DestroyMined))]
	public class Patch_Destroy
	{
		static public void Prefix(Mineable __instance)
		{	
			Filth rocks = __instance.Map?.thingGrid.ThingsListAtFast(__instance.Position).FirstOrDefault(x => x.def == Owl_Filth_Rocks) as Filth;
			rocks?.ThickenFilth();
		}
    }

	//When a new building is spawned, we check the adjacent cells to see if it created an impassible situtation that makes filth unreachable, which could otherwise cause the standing bug
	[HarmonyPatch(typeof(Building), nameof(Building.SpawnSetup))]
	public class Patch_SpawnSetup
	{
		static public void Postfix(Building __instance)
		{
			Map map = __instance?.Map;

			if (map?.AgeInDays != 0 && __instance.Position != null)
			{
				MapComponent_ScatteredStones comp = map.GetComponent<MapComponent_ScatteredStones>();
				
				__instance.OccupiedRect().AdjacentCellsCardinal.ToList().ForEach
				(x =>
					{
						if (x.InBounds(map) && x.Impassable(map)) comp.ValidateCell(x, true);
					}
				);
			}
		}
    }

	//When something is mined, place filth there
	[HarmonyPatch(typeof(Mineable), nameof(Mineable.TrySpawnYield))]
	public class Patch_TrySpawnYield
	{
		static public void Postfix(Mineable __instance, Map map)
		{
			if (minedFilth && (map.thingGrid.ThingsAt(__instance.Position)?.Any(x => x.def?.thingCategories?.Contains(ResourceBank.ThingCategoryDefOf.StoneChunks) ?? false) ?? false))
			{
				Thing rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null);
				//Place underneath chunk
				GenPlace.TryPlaceThing(rocks, __instance.Position, map, ThingPlaceMode.Direct);
				//Match color
				rocks.DrawColor = ((Rocks)rocks).MatchColor(__instance);
			}
		}
	}
}
