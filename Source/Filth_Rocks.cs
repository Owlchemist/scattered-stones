using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;
using static ScatteredStones.Mod_ChunkyChunks;
using static ScatteredStones.ResourceBank.ThingDefOf;


using System.Diagnostics;


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
            else if (this.Position != null)
            {
                var stoneChunkHere = this.Map.thingGrid.ThingsListAtFast(this.Position).FirstOrDefault(x => stoneChunks.Contains(x.def));
                if (stoneChunkHere != null) return stoneChunkHere.DrawColor;
            }
            //Default
            return ChunkGranite.graphicData.color;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {			
			//If the calling methods is a carry drop-off event, or a map refund (happens because it thinks filth shouldn't be under chunks), then ignore. Bit of a hack doing it this way, but it'll until a better method is devised.
			var method = new StackFrame(2).GetMethod().Name;
			if (method.Contains("WipeExistingThings") || method.Contains("Refund")) return;
			
			//This happens when filth despawns due to age, but we reset its timer if it's still underneath a rock
			if (this.TicksSinceThickened >= this.DisappearAfterTicks)
            {
				var cell = this.Map.thingGrid.ThingsListAtFast(this.Position).Select(x => x.def);
				bool flag = cell.Intersect(stoneChunks).Any() || cell.Intersect(stoneCliff).Any();
                //Reset if true
				if (flag)
				{
					this.ThickenFilth();
					return;
				}
                //Else delete as normal
				else this.DeSpawn(mode);
            }
        }
	}   
}