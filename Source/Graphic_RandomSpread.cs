using Verse;
using UnityEngine;
using RimWorld;
using System.Collections.Generic;

namespace ScatteredStones
{
    ///<summary>
	///Same as graphic random, but allows random sizes too
	///</summary>
	public class Graphic_RandomSpread : Graphic_Random
	{
		private Dictionary<string, float[]> sessionCache = new Dictionary<string, float[]>();

		public override void Print(SectionLayer layer, Thing thing, float extraRotation)
		{
			float sizeMultiplier = 1;
			float randomRotation = 0;
			float offsetX = 0;
			float offsetY = 0;

			if (!sessionCache.ContainsKey(thing.ThingID))
			{
				if (thing.ThingID != null && thing.def.HasModExtension<RandomDraw>())
				{
					//fakeSeed = thing.Position.x + thing.Position.z;
					int fakeSeed = thing.Position.GetHashCode();
					var randomDraw = thing.def.GetModExtension<RandomDraw>();
					randomRotation = Rand.RangeInclusiveSeeded(0, 360, fakeSeed);
					sizeMultiplier = Rand.RangeSeeded(randomDraw.minSize, randomDraw.maxSize, fakeSeed);
					offsetX = Rand.RangeSeeded(0 - randomDraw.offsetRange, 0 + randomDraw.offsetRange, fakeSeed);
					offsetY = Rand.RangeSeeded(0 - randomDraw.offsetRange, 0 + randomDraw.offsetRange, fakeSeed);
				}
				
				sessionCache.Add(thing.ThingID, new float[] { sizeMultiplier, randomRotation, offsetX, offsetY });
			}
			
			//Apply size
			Vector2 size = this.drawSize * sessionCache[thing.ThingID][0];

			//Apply rotation
			float rotation = sessionCache[thing.ThingID][1];
			
			Vector3 center = thing.TrueCenter() + this.DrawOffset(thing.Rotation) + new Vector3(sessionCache[thing.ThingID][2], 0, sessionCache[thing.ThingID][3]);
			Material mat = this.MatAt(thing.Rotation, thing);
			Vector2[] uvs;
			Color32 color;
			Graphic.TryGetTextureAtlasReplacementInfo(mat, thing.def.category.ToAtlasGroup(), false, true, out mat, out uvs, out color);
			Printer_Plane.PrintPlane(layer, center, size, mat, rotation, false, uvs, new Color32[]
			{
				color,
				color,
				color,
				color
			}, 0.01f, 0f);
			if (this.ShadowGraphic != null && thing != null)
			{
				this.ShadowGraphic.Print(layer, thing, 0f);
			}
		}
	}
}