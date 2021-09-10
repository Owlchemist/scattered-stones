using Verse;
using RimWorld;
using System.Linq;
using static ScatteredStones.ResourceBank.ThingDefOf;
using static ScatteredStones.Mod_ChunkyChunks;

namespace ScatteredStones
{
	public class MapComponent_ChunkyChunks : MapComponent
	{
		
        bool applied = false;
        public MapComponent_ChunkyChunks(Map map) : base(map) {}

        public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.applied, "applied", false, false);
		}

        public override void FinalizeInit()
        {
            //This should only ever run once per map.
            if (!applied)
            {
                //Go through all the chunks on this map so we can place the rock graphics beneath them.
                var localStone = map.listerThings.AllThings.Where(x => (!x.IsInAnyStorage() && stoneChunks.Contains(x.def)) || (stoneCliff.Contains(x.def) && !x.Fogged())).ToList();
                foreach(var stone in localStone)
                {
                    var rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null);
                    //Place underneath chunk
                    GenPlace.TryPlaceThing(rocks, stone.Position, map, ThingPlaceMode.Direct);
                    //Match color
                    rocks.DrawColor = rocks.ChangeType<Rocks>().MatchColor(stone);
                }
                applied = true;
            }
        }
    }
}