using Verse;
using RimWorld;
using System.Linq;
using static ScatteredStones.ResourceBank.ThingDefOf;
using static ScatteredStones.ScatteredStonesUtility;
using static ScatteredStones.ModSettings_ScatteredStones;

namespace ScatteredStones
{
	public class MapComponent_ScatteredStones : MapComponent
	{
        bool applied = false;
        public MapComponent_ScatteredStones(Map map) : base(map) {}

        public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.applied, "applied", false, false);
		}

        public override void FinalizeInit()
        {
            //This should only ever run once per map.
            if (!applied)
            {
                map.listerThings.AllThings.ToList().ForEach
                (x =>
                    {
                        if
                        (
                            //Is stone chunk, and not in storage? && Is on water and allowed?
                            (!x.IsInAnyStorage() && stoneChunks.Contains(x.def) && (allowOnWater || (!map.terrainGrid.TerrainAt(x.Position)?.IsWater ?? false))) ||
                            //Is reachable cliff that's not fogged?
                            (stoneCliff.Contains(x.def) && !x.Fogged() && ValidateCell(x.Position, false)) && 
                            //Is not along the map edge?
                            GenAdjFast.AdjacentCellsCardinal(x.Position).Where(x => x.InBounds(map)).Count() == 4
                        )
                        {
                            Thing rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null);
                            //Place underneath chunk
                            GenPlace.TryPlaceThing(rocks, x.Position, map, ThingPlaceMode.Direct);
                            //Match color
                            rocks.DrawColor = ((Rocks)rocks).MatchColor(x);
                        }
                    }
                );
                applied = true;
            }
        }

        public bool ValidateCell(IntVec3 pos, bool autoClean = false)
        {
            //Cache the local filth here to check if we even need to bother processing this cell
            var localFilth = map.thingGrid.ThingsListAtFast(pos).Where(x => x.def == Owl_Filth_Rocks)?.ToList();
            if (autoClean && localFilth?.Count == 0) return true;

            //Fetch addjacent cells and proceess them
            var adjacentCells = GenAdjFast.AdjacentCellsCardinal(pos).Where(x => x.InBounds(map));
            int i = 0;
            foreach (var cell in adjacentCells)
            {
                //Check for passibility and water...
                if(map.terrainGrid.TerrainAt(cell)?.IsWater == true)
                {
                    i = 4;
                    break;
                }
                //Add a "point" if conditions are met.
                if (map.thingGrid.ThingsListAtFast(cell)?.FirstOrDefault(y => y.def?.fillPercent == 1 || y.def?.passability == Traversability.Impassable) != null) i++;
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