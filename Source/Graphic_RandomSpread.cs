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
			float sizeMultiplier = 1, randomRotation = 0, offsetX = 0, offsetY = 0;

			if (!sessionCache.ContainsKey(thing.thingIDNumber))
			{
				if (thing.def.HasModExtension<RandomDraw>())
				{
					int fakeSeed = thing.Position.GetHashCode();
					RandomDraw randomDraw = thing.def.GetModExtension<RandomDraw>();
					randomRotation = Rand.RangeInclusiveSeeded(0, 360, fakeSeed);
					sizeMultiplier = Rand.RangeSeeded(randomDraw.minSizeModified, randomDraw.maxSizeModified, fakeSeed);
					offsetX = Rand.RangeSeeded(0 - randomDraw.offsetRangeModified, 0 + randomDraw.offsetRangeModified, fakeSeed);
					offsetY = Rand.RangeSeeded(0 - randomDraw.offsetRangeModified, 0 + randomDraw.offsetRangeModified, fakeSeed);
				}
				
				sessionCache.Add(thing.thingIDNumber, new float[] { sizeMultiplier, randomRotation, offsetX, offsetY });
			}
			
			//Check if on a storage building
			if (thing.Map.thingGrid.ThingAt<Building_Storage>(thing.Position) != null)
			{
				sessionCache[thing.thingIDNumber][2] = sessionCache[thing.thingIDNumber][3] = 0f;
			}
			
			Vector3 center = thing.positionInt.ToVector3ShiftedWithAltitude(thing.def.altitudeLayer) + DrawOffset(thing.rotationInt) + new Vector3(sessionCache[thing.thingIDNumber][2], 0, sessionCache[thing.thingIDNumber][3]);
			Material mat = MatAt(thing.rotationInt, thing);
			Graphic.TryGetTextureAtlasReplacementInfo(mat, TextureAtlasGroup.Item, false, true, out mat, out Vector2[] uvs, out Color32 color);
			Printer_Plane.PrintPlane(layer, center, drawSize * sessionCache[thing.thingIDNumber][0], mat, sessionCache[thing.thingIDNumber][1], false, uvs, new Color32[]
			{
				color,
				color,
				color,
				color
			}, 0.01f, 0f);
			
			ShadowGraphic?.Print(layer, thing, 0f);
		}
	}
}