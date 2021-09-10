
using System.Linq;
using HarmonyLib;
using RimWorld;
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
}
