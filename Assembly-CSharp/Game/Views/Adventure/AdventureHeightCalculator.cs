using System;
using System.Collections.Generic;
using GameData.Adventure;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C67 RID: 3175
	public static class AdventureHeightCalculator
	{
		// Token: 0x0600A19B RID: 41371 RVA: 0x004B8698 File Offset: 0x004B6898
		public static AdventureHeightCalculator.HeightResult Calculate(float[] heights)
		{
			return AdventureHeightCalculator.Calculate(heights, 100f);
		}

		// Token: 0x0600A19C RID: 41372 RVA: 0x004B86B8 File Offset: 0x004B68B8
		public static AdventureHeightCalculator.HeightResult Calculate(float[] heights, float maxValue)
		{
			bool flag = heights == null || heights.Length != 9;
			AdventureHeightCalculator.HeightResult result;
			if (flag)
			{
				Debug.LogWarning(string.Format("[AdventureHeightCalculator] Invalid heights array, expected {0} elements", 9));
				float[] defaultMicroHeights = new float[9];
				for (int i = 0; i < 9; i++)
				{
					defaultMicroHeights[i] = 0.5f;
				}
				result = new AdventureHeightCalculator.HeightResult(0.5f, defaultMicroHeights);
			}
			else
			{
				float[] sorted = new float[9];
				Array.Copy(heights, sorted, 9);
				Array.Sort<float>(sorted);
				float median = sorted[4];
				float volumeHeight = Mathf.Clamp01(median / maxValue);
				float[] microHeights = new float[9];
				for (int j = 0; j < 9; j++)
				{
					microHeights[j] = AdventureHeightCalculator.CalculateMicroHeight(heights[j], median);
				}
				result = new AdventureHeightCalculator.HeightResult(volumeHeight, microHeights);
			}
			return result;
		}

		// Token: 0x0600A19D RID: 41373 RVA: 0x004B8794 File Offset: 0x004B6994
		public static AdventureHeightCalculator.HeightResult CalculateNormalized(float[] heights)
		{
			return AdventureHeightCalculator.Calculate(heights, 100f);
		}

		// Token: 0x0600A19E RID: 41374 RVA: 0x004B87B4 File Offset: 0x004B69B4
		private static float CalculateMicroHeight(float height, float median)
		{
			bool flag = Mathf.Approximately(height, median);
			float result;
			if (flag)
			{
				result = 0.5f;
			}
			else
			{
				float delta = height - median;
				float mappingDelta = Mathf.Clamp(Mathf.Abs(delta), 0f, 2.2282608f);
				float microHeight = (delta < 0f) ? (0.5f - 0.5f * mappingDelta / 2.2282608f) : (0.5f + 0.5f * mappingDelta / 2.2282608f);
				result = Mathf.Clamp01(microHeight);
			}
			return result;
		}

		// Token: 0x0600A19F RID: 41375 RVA: 0x004B8830 File Offset: 0x004B6A30
		public static float[] ExtractHeights<T>(IEnumerable<T> blocks, int volumeX, int volumeY, Func<T, AdventureBlockIndex> indexGetter, Func<T, float> heightGetter)
		{
			float[] heights = new float[9];
			bool flag = blocks == null;
			float[] result;
			if (flag)
			{
				result = heights;
			}
			else
			{
				foreach (T block in blocks)
				{
					AdventureBlockIndex index = indexGetter(block);
					bool flag2;
					if (index.X == volumeX && index.Y == volumeY)
					{
						int i = index.I;
						flag2 = (i >= 0 && i < 9);
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag3)
					{
						heights[index.I] = heightGetter(block);
					}
				}
				result = heights;
			}
			return result;
		}

		// Token: 0x04007D7A RID: 32122
		public const int MicroCount = 9;

		// Token: 0x04007D7B RID: 32123
		public const float MaxHeightValue = 100f;

		// Token: 0x04007D7C RID: 32124
		public const float MaxMicroHeightValue = 2.2282608f;

		// Token: 0x04007D7D RID: 32125
		private const float ArtHeight = 1840f;

		// Token: 0x04007D7E RID: 32126
		private const float MicroArtHeight = 82f;

		// Token: 0x0200239E RID: 9118
		public readonly struct HeightResult
		{
			// Token: 0x060103F9 RID: 66553 RVA: 0x00658C24 File Offset: 0x00656E24
			public HeightResult(float volumeHeight, float[] microHeights)
			{
				this.VolumeHeight = volumeHeight;
				this.MicroHeights = microHeights;
			}

			// Token: 0x0400DF68 RID: 57192
			public readonly float VolumeHeight;

			// Token: 0x0400DF69 RID: 57193
			public readonly float[] MicroHeights;
		}
	}
}
