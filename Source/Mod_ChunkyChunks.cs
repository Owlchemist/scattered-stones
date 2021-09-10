using HarmonyLib;
using Verse;
using System.Collections.Generic;

namespace ScatteredStones
{
	public class Mod_ChunkyChunks : Mod
	{
		public Mod_ChunkyChunks(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
		}

		static public IEnumerable<ThingDef> stoneChunks = new List<ThingDef>();
		static public IEnumerable<ThingDef> stoneCliff = new List<ThingDef>();
    }
}