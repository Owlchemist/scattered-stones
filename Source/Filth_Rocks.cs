using RimWorld;
using UnityEngine;
using Verse;
using System.Diagnostics;
using static ScatteredStones.ScatteredStonesUtility;
using static ScatteredStones.ResourceBank.ThingDefOf;

namespace ScatteredStones
{
    //Normally you can't recolor filth because it doesn't support ThingComps. This derived class only exists to allow dynamic colors.
	public class Rocks : Filth
	{
        public override Color DrawColor
        {
            get
            {
                if (this.color.a == 0f) this.color = this.MatchColor(null);
                return this.color;
            }
            set { this.color = value; }
        }

        private Color color;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Color>(ref this.color, "color", ChunkGranite.graphicData.color, false);
        }

        public Color MatchColor(Thing matchToThis)
        {
            if (matchToThis != null)
            {
                return matchToThis.DrawColor;
            }
            else if (this.positionInt != IntVec3.Invalid)
            {
                var list = this.Map.thingGrid.ThingsListAtFast(this.Position);
                var length = list.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = list[i];
                    if (stoneChunks.Contains(item.def.index))
                    {
                        return item.DrawColor;
                    }
                }
            }
            //Default
            return ChunkGranite.graphicData.color;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
			//This happens when filth despawns due to age, but we reset its timer if it's still underneath a rock
			if (ModSettings_ScatteredStones.neverDespawn && this.TicksSinceThickened >= this.DisappearAfterTicks)
            {
                var list = this.Map.thingGrid.ThingsListAtFast(this.Position);
                var length = list.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = list[i];
                    //Reset if true
                    if (stoneChunks.Contains(item.def.index) || stoneCliff.Contains(item.def.index))
                    {
                        this.ThickenFilth();
                        return;
                    }
                }
            }
            base.Destroy(mode);
        }
	}   
}