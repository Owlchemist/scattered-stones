using HarmonyLib;
using Verse;
using UnityEngine;
using System;
using static ScatteredStones.ModSettings_ScatteredStones;
using static ScatteredStones.ScatteredStonesUtility;

namespace ScatteredStones
{
	public class Mod_ScatteredStones : Mod
	{
		public Mod_ScatteredStones(ModContentPack content) : base(content)
		{
			base.GetSettings<ModSettings_ScatteredStones>();
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			LongEventHandler.QueueLongEvent(() => Setup(), null, false, null);
		}

 		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard options = new Listing_Standard();
			options.Begin(inRect);
			options.Label("ScatteredStones.Settings.ScaleSliderMin".Translate("1", "0.5", "2") + Math.Round(minScaleModifier, 2), -1f, null);
			minScaleModifier = options.Slider(minScaleModifier, 0.5f, 2f);

			options.Label("ScatteredStones.Settings.ScaleSliderMax".Translate("1", "0.5", "2") + Math.Round(maxScaleModifier, 2), -1f, null);
			maxScaleModifier = options.Slider(maxScaleModifier, 0.5f, 2f);

			options.Label("ScatteredStones.Settings.OffsetSlider".Translate("1", "0" ,"3") + Math.Round(offsetModifier, 2), -1f, null);
			offsetModifier = options.Slider(offsetModifier, 0f, 3f);

			options.CheckboxLabeled("ScatteredStones.Settings.AllowOnWater".Translate(), ref allowOnWater, "ScatteredStones.Settings.AllowOnWater.Desc".Translate());
			options.CheckboxLabeled("ScatteredStones.Settings.NeverDespawn".Translate(), ref neverDespawn, "ScatteredStones.Settings.NeverDespawn.Desc".Translate());
			options.CheckboxLabeled("ScatteredStones.Settings.MinedFilth".Translate(), ref minedFilth, "ScatteredStones.Settings.MinedFilth.Desc".Translate());

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
			UpdateModifiers(true);
		}
	}
	public class ModSettings_ScatteredStones : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look<float>(ref minScaleModifier, "minScaleModifier", 1f);
			Scribe_Values.Look<float>(ref maxScaleModifier, "maxScaleModifier", 1f);
			Scribe_Values.Look<float>(ref offsetModifier, "offsetModifier", 1f);
			Scribe_Values.Look<bool>(ref allowOnWater, "allowOnWater", true);
			Scribe_Values.Look<bool>(ref neverDespawn, "neverDespawn");
			Scribe_Values.Look<bool>(ref minedFilth, "minedFilth", true);
			base.ExposeData();
		}

		static public float minScaleModifier = 1f, maxScaleModifier = 1f, offsetModifier = 1f;
		static public bool allowOnWater = true, minedFilth = true, neverDespawn;
	}
}
