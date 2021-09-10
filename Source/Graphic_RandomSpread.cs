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
			Vector2 size;
			RandomDraw randomDraw;
			float sizeMultiplier = 1;
			float randomRotation = 0;
			float offsetX = 0;
			float offsetY = 0;
			if (!sessionCache.ContainsKey(thing.ThingID))
			{
				if (thing.ThingID != null && thing.def.HasModExtension<RandomDraw>())
				{
					randomDraw = thing.def.GetModExtension<RandomDraw>();
					sizeMultiplier = Rand.Range(randomDraw.minSize, randomDraw.maxSize);
					randomRotation = Rand.RangeInclusive(0, 360);
					offsetX = Rand.Range(0 - randomDraw.offsetRange, 0 + randomDraw.offsetRange);
					offsetY = Rand.Range(0 - randomDraw.offsetRange, 0 + randomDraw.offsetRange);
				}
				
				sessionCache.Add(thing.ThingID, new float[] { sizeMultiplier, randomRotation, offsetX, offsetY });
			}
			
			bool flag;
			if (this.ShouldDrawRotated)
			{
				size = this.drawSize;
				flag = false;
			}
			else
			{
				if (!thing.Rotation.IsHorizontal) size = this.drawSize;
				else size = this.drawSize.Rotated();

				flag = ((thing.Rotation == Rot4.West && this.WestFlipped) || (thing.Rotation == Rot4.East && this.EastFlipped));
			}
			//Apply size
			size *= sessionCache[thing.ThingID][0];

			//Apply rotation
			float rotation = sessionCache[thing.ThingID][1] + extraRotation;
			if (flag && this.data != null) rotation += this.data.flipExtraRotation;

			
			Vector3 center = thing.TrueCenter() + this.DrawOffset(thing.Rotation) + new Vector3(sessionCache[thing.ThingID][2], 0, sessionCache[thing.ThingID][3]);

			Material mat = this.MatAt(thing.Rotation, thing);
			Vector2[] uvs;
			Color32 color;
			Graphic.TryGetTextureAtlasReplacementInfo(mat, thing.def.category.ToAtlasGroup(), flag, true, out mat, out uvs, out color);
			Printer_Plane.PrintPlane(layer, center, size, mat, rotation, flag, uvs, new Color32[]
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