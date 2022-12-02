
using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using static ScatteredStones.ResourceBank.ThingDefOf;
using static ScatteredStones.ModSettings_ScatteredStones;
using static ScatteredStones.ScatteredStonesUtility;

namespace ScatteredStones
{
	//Unlike the filth under the rock chunks, the ones under mineables never get their timers reset. To prevent despawning immediately, we do the reset upon mining.
	[HarmonyPatch(typeof(Mineable), nameof(Mineable.DestroyMined))]
	public class Patch_Destroy
	{
		static public void Prefix(Mineable __instance)
		{
			var list = __instance.Map?.thingGrid.ThingsListAtFast(__instance.Position);
			var length = list.Count;
			for (int i = 0; i < length; i++)
			{
				var item = list[i];
				if (item.def == Owl_Filth_Rocks) ((Filth)item).ThickenFilth();
			}
		}
    }

	//When a new building is spawned, we check the adjacent cells to see if it created an impassible situtation that makes filth unreachable, which could otherwise cause the standing bug
	[HarmonyPatch(typeof(Building), nameof(Building.SpawnSetup))]
	public class Patch_SpawnSetup
	{
		static public void Postfix(Map map, Building __instance)
		{
			//Age in days to filter out newly generating maps
			if (__instance.def.passability != Traversability.Standable && map?.AgeInDays != 0 && __instance.positionInt != IntVec3.Invalid)
			{
				//Collection gets modified, need a working list copy
				IntVec3[] list = GenAdjFast.AdjacentCellsCardinal(__instance).ToArray();
				for (int i = 0; i < list.Length; i++)
				{
					var cell = list[i];
					if (cell.InBounds(map) && !cell.Standable(map)) ScatteredStonesUtility.ValidateCell(cell, map, true);
				}
			}
		}
    }

	//When something is mined, place filth there
	[HarmonyPatch(typeof(Mineable), nameof(Mineable.TrySpawnYield))]
	public class Patch_TrySpawnYield
	{
		static public void Postfix(Mineable __instance, Map map)
		{
			if (!minedFilth) return;

			//Collection gets modified, need a working list copy
			List<Thing> list = map.thingGrid.ThingsListAtFast(__instance.positionInt);
			var length = list.Count;
			for (int i = 0; i < length; i++)
			{
				var item = list[i];
				if (stoneChunks.Contains(item.def.index))
				{
					Rocks rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null) as Rocks;
					//Place underneath chunk
					GenPlace.TryPlaceThing(rocks, __instance.Position, map, ThingPlaceMode.Direct);
					//Match color
					rocks.DrawColor = rocks.MatchColor(__instance);
				}
			}
		}
	}

	//Makes it so that placing something ontop of a chunk does not wipe it like it normally would
	[HarmonyPatch(typeof(GenSpawn), nameof(GenSpawn.SpawningWipes))]
	public class Patch_SpawningWipes
	{
		static public bool Postfix(bool __result, BuildableDef newEntDef, BuildableDef oldEntDef)
		{
			if (oldEntDef == ResourceBank.ThingDefOf.Owl_Filth_Rocks)
			{
				var index = newEntDef?.index ?? 0;
				if (stoneChunks.Contains(index) || stoneCliff.Contains(index)) return false;
			}
			 
			return __result;
		}
	}

}
