using HarmonyLib;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static ScatteredStones.ModSettings_ScatteredStones;

namespace ScatteredStones
{
	public class Mod_ScatteredStones : Mod
	{
		public Mod_ScatteredStones(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			base.GetSettings<ModSettings_ScatteredStones>();
		}

		static public IEnumerable<ThingDef> stoneChunks = new List<ThingDef>();
		static public IEnumerable<ThingDef> stoneCliff = new List<ThingDef>();
		public static bool settingsChanged = false;

 		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard options = new Listing_Standard();
			options.Begin(inRect);
			options.Label("Min scale modifier (Mod default: 1, Min: 0.5, Max: 2): " + Math.Round(minScaleModifier, 2), -1f, null);
			minScaleModifier = options.Slider(minScaleModifier, 0.5f, 2f);

			options.Label("Max scale modifier (Mod default: 1, Min: 0.5, Max: 2): " + Math.Round(maxScaleModifier, 2), -1f, null);
			maxScaleModifier = options.Slider(maxScaleModifier, 0.5f, 2f);

			options.Label("Offset modifier (Mod default: 1, Min: 0, Max: 3): " + Math.Round(offsetModifier, 2), -1f, null);
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
			Mod_ScatteredStones.settingsChanged = true;
			UpdateModifiers();
		}

		public static void UpdateModifiers()
		{
			var dd = DefDatabase<ThingDef>.AllDefs.Where(x => x.HasModExtension<RandomDraw>())?.ToList();
			foreach (var def in dd)
			{
				var modX = def.GetModExtension<RandomDraw>();
				modX.minSizeModified = modX.minSize * minScaleModifier;
				modX.maxSizeModified = Mathf.Clamp(modX.maxSize * maxScaleModifier, modX.minSizeModified, 10);
				modX.offsetRangeModified = modX.offsetRange * offsetModifier;

				//Let the caches refresh.
				if (Mod_ScatteredStones.settingsChanged) def.graphic.ChangeType<Graphic_RandomSpread>().sessionCache.Clear();
			}
			if (Find.CurrentMap != null) Find.CurrentMap.mapDrawer.WholeMapChanged(MapMeshFlag.Things);
			Mod_ScatteredStones.settingsChanged = false;
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
