using Verse;
using RimWorld;
using System.Linq;
using static ScatteredStones.ResourceBank.ThingDefOf;
using static ScatteredStones.Mod_ScatteredStones;

namespace ScatteredStones
{
	public class MapComponent_ScatteredStones : MapComponent
	{
		
        bool applied = false;
        bool fixApplied = false;
        public MapComponent_ScatteredStones(Map map) : base(map) {}

        public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.applied, "applied", false, false);
            Scribe_Values.Look<bool>(ref this.fixApplied, "fixApplied", false, false);
		}

        public override void FinalizeInit()
        {
            //This should only ever run once per map.
            if (!applied)
            {
                //Go through all the chunks on this map so we can place the rock graphics beneath them.
                var localStone = map.listerThings.AllThings.Where(x => (!x.IsInAnyStorage() && stoneChunks.Contains(x.def)) || (stoneCliff.Contains(x.def) && !x.Fogged() && ValidateCell(x.Position, false))).ToList();
                foreach(var stone in localStone)
                {
                    var rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null);
                    //Place underneath chunk
                    GenPlace.TryPlaceThing(rocks, stone.Position, map, ThingPlaceMode.Direct);
                    //Match color
                    rocks.DrawColor = rocks.ChangeType<Rocks>().MatchColor(stone);
                }
                applied = true;
                fixApplied = true; //Don't need a fix for a fresh install
            }
            if (!fixApplied) TempFix(); 
        }

        //Hotfix to remove existing junk from early adopter's saves. We can get rid of this in a few weeks.
        private void TempFix()
        {
            if (Prefs.DevMode) Log.Message("[Scattered Stones] Running one-time cleanup...");
            var localRockFilth = map.listerThings.AllThings.Where(x => x.def == Owl_Filth_Rocks).ToList();
            foreach (var filth in localRockFilth)
            {   
                var adjacentCells = GenAdjFast.AdjacentCellsCardinal(filth).Where(x => x.InBounds(map));
                var i = 0;
                foreach (var cell in adjacentCells)
                {
                    if(map.thingGrid.ThingsListAtFast(cell)?.FirstOrDefault(y => y.def?.fillPercent == 1 || y.def?.passability == Traversability.Impassable) != null) i++;
                    if(map.terrainGrid.TerrainAt(cell)?.IsWater == true) i += 4;
                }
                if (i > 3) filth.DeSpawn();
            }
            fixApplied = true;
        }

        public bool ValidateCell(IntVec3 pos, bool autoClean = false)
        {
            //Cache the local filth here to check if we even need to bother processing this cell
            var localFilth = map.thingGrid.ThingsListAtFast(pos).Where(x => x.def == Owl_Filth_Rocks)?.ToList();
            if (autoClean && localFilth?.Count() == 0) return true;

            //Fetch addjacent cells and proceess them
            var adjacentCells = GenAdjFast.AdjacentCellsCardinal(pos).Where(x => x.InBounds(map));
            var i = 0;
            foreach (var cell in adjacentCells)
            {
                //Check for passibility and water...
                if(map.terrainGrid.TerrainAt(cell)?.IsWater == true)
                {
                    i = 4;
                    break;
                }
                if(map.thingGrid.ThingsListAtFast(cell)?.FirstOrDefault(y => y.def?.fillPercent == 1 || y.def?.passability == Traversability.Impassable) != null) i++;
            }
            //Check the score, delete/false if more than 3
            if (i > 3) 
            {
                if (autoClean) localFilth?.ForEach(y => y.DeSpawn());
                return false;
            }
            return true;
        }
    }
}