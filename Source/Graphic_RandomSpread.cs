using Verse;
using UnityEngine;
using RimWorld;
using System.Collections.Generic;

namespace ScatteredStones
{
	public class Graphic_RandomSpread : Graphic_Random
	{
		public Dictionary<int, float[]> sessionCache = new Dictionary<int, float[]>();

		public override void Print(SectionLayer layer, Thing thing, float extraRotation)
		{
			float sizeMultiplier = 1;
			float randomRotation = 0;
			float offsetX = 0;
			float offsetY = 0;

			if (!sessionCache.ContainsKey(thing.thingIDNumber))
			{
				if (thing.ThingID != null && thing.def.HasModExtension<RandomDraw>())
				{
					//fakeSeed = thing.Position.x + thing.Position.z;
					int fakeSeed = thing.Position.GetHashCode();
					var randomDraw = thing.def.GetModExtension<RandomDraw>();
					randomRotation = Rand.RangeInclusiveSeeded(0, 360, fakeSeed);
					sizeMultiplier = Rand.RangeSeeded(randomDraw.minSizeModified, randomDraw.maxSizeModified, fakeSeed);
					offsetX = Rand.RangeSeeded(0 - randomDraw.offsetRangeModified, 0 + randomDraw.offsetRangeModified, fakeSeed);
					offsetY = Rand.RangeSeeded(0 - randomDraw.offsetRangeModified, 0 + randomDraw.offsetRangeModified, fakeSeed);
				}
				
				sessionCache.Add(thing.thingIDNumber, new float[] { sizeMultiplier, randomRotation, offsetX, offsetY });
			}
			
			//Apply size
			Vector2 size = this.drawSize * sessionCache[thing.thingIDNumber][0];

			//Apply rotation
			float rotation = sessionCache[thing.thingIDNumber][1];
			
			Vector3 center = thing.TrueCenter() + this.DrawOffset(thing.Rotation) + new Vector3(sessionCache[thing.thingIDNumber][2], 0, sessionCache[thing.thingIDNumber][3]);
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