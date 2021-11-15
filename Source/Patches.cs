
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using static ScatteredStones.ResourceBank.ThingDefOf;

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
}
