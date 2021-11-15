using Verse;
using UnityEngine;
using System.Linq;
using static ScatteredStones.ModSettings_ScatteredStones;
using static ScatteredStones.ResourceBank.ThingCategoryDefOf;

namespace ScatteredStones
{
	public static class ScatteredStonesUtility
	{
		public static ThingDef[] stoneChunks;
		public static ThingDef[] stoneCliff;

		public static void Setup()
		{
			stoneChunks = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.thingCategories?.Contains(StoneChunks) ?? false).ToArray();
            stoneCliff = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => (x.building?.isNaturalRock ?? false) && !x.building.isResourceRock).ToArray();
            UpdateModifiers();
		}

		public static void UpdateModifiers(bool settingsChanged = false)
		{
			var dd = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.HasModExtension<RandomDraw>());
			foreach (var def in dd)
			{
				RandomDraw modX = def.GetModExtension<RandomDraw>();
				modX.minSizeModified = modX.minSize * minScaleModifier;
				modX.maxSizeModified = Mathf.Clamp(modX.maxSize * maxScaleModifier, modX.minSizeModified, 10);
				modX.offsetRangeModified = modX.offsetRange * offsetModifier;

				//Let the caches refresh.
				if (settingsChanged) ((Graphic_RandomSpread)def.graphic).sessionCache.Clear();
			}
			if (Current.ProgramState == ProgramState.Playing) Find.CurrentMap?.mapDrawer.WholeMapChanged(MapMeshFlag.Things);
		}
	}
}
