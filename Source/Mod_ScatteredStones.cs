using HarmonyLib;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static ScatteredStones.ModSettings_ScatteredStones;
using static ScatteredStones.ResourceBank.ThingCategoryDefOf;

namespace ScatteredStones
{
	public class Mod_ScatteredStones : Mod
	{
		static public ThingDef[] stoneChunks;
		static public ThingDef[] stoneCliff;
		public static bool settingsChanged = false;

		public Mod_ScatteredStones(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			base.GetSettings<ModSettings_ScatteredStones>();
			LongEventHandler.QueueLongEvent(() => Setup(), "Mod_ScatteredStones.Setup", false, null);
		}

		private void Setup()
		{
			stoneChunks = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.thingCategories != null && x.thingCategories.Contains(StoneChunks)).ToArray();
            stoneCliff = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.building != null && x.building.isNaturalRock && !x.building.isResourceRock).ToArray();
            UpdateModifiers();
		}

 		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard options = new Listing_Standard();
			options.Begin(inRect);
			options.Label("ScatteredStones_ScaleSliderMin".Translate("1", "0.5", "2") + Math.Round(minScaleModifier, 2), -1f, null);
			minScaleModifier = options.Slider(minScaleModifier, 0.5f, 2f);

			options.Label("ScatteredStones_ScaleSliderMax".Translate("1", "0.5", "2") + Math.Round(maxScaleModifier, 2), -1f, null);
			maxScaleModifier = options.Slider(maxScaleModifier, 0.5f, 2f);

			options.Label("ScatteredStones_OffsetSlider".Translate("1", "0" ,"3") + Math.Round(offsetModifier, 2), -1f, null);
			offsetModifier = options.Slider(offsetModifier, 0f, 3f);

			options.End();
			base.DoSettingsWindowContents(inRect);
		}
		 public override string SettingsCategory()
		{
			return "Scattered Stones";
		}

		public override void WriteSettings()
		{
			base.WriteSettings();
			settingsChanged = true;
			UpdateModifiers();
		}

		public static void UpdateModifiers()
		{
			var dd = DefDatabase<ThingDef>.AllDefs.Where(x => x.HasModExtension<RandomDraw>());
			foreach (var def in dd)
			{
				var modX = def.GetModExtension<RandomDraw>();
				modX.minSizeModified = modX.minSize * minScaleModifier;
				modX.maxSizeModified = Mathf.Clamp(modX.maxSize * maxScaleModifier, modX.minSizeModified, 10);
				modX.offsetRangeModified = modX.offsetRange * offsetModifier;

				//Let the caches refresh.
				if (settingsChanged) def.graphic.ChangeType<Graphic_RandomSpread>().sessionCache.Clear();
			}
			if (Find.CurrentMap != null) Find.CurrentMap.mapDrawer.WholeMapChanged(MapMeshFlag.Things);
			settingsChanged = false;
		}
	}
	public class ModSettings_ScatteredStones : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look<float>(ref minScaleModifier, "minScaleModifier", 1, false);
			Scribe_Values.Look<float>(ref maxScaleModifier, "maxScaleModifier", 1, false);
			Scribe_Values.Look<float>(ref offsetModifier, "offsetModifier", 1, false);
			base.ExposeData();
		}

		static public float minScaleModifier = 1f;
		static public float maxScaleModifier = 1f;
		static public float offsetModifier = 1f;
	}
}
